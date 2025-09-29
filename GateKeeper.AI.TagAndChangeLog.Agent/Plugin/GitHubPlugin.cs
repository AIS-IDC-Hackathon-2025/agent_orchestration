using GateKeeper.AI.Shared;
using GateKeeper.AI.Shared.Models;
using Microsoft.SemanticKernel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static GateKeeper.AI.Shared.Settings;
//using GitHubModels = GateKeeper.AI.Shared.Models;
namespace GateKeeper.AI.TagAndChangeLog.Agent.Plugin;



public sealed class GitHubPlugin(Settings settings)
{
    [KernelFunction]
    [Description("Get the authenticated user's profile information")]
    public async Task<GitHubModels.User> GetUserProfileAsync()
    {
        using HttpClient client = this.CreateClient();
        JsonDocument response = await MakeRequestAsync(client, "/user");
        return response.Deserialize<GitHubModels.User>() ?? throw new InvalidDataException($"Request failed: {nameof(GetUserProfileAsync)}");
    }

    [KernelFunction]
    [Description("Get a repository's details")]
    public async Task<GitHubModels.Repo> GetRepositoryAsync(string organization, string repo)
    {
        using HttpClient client = this.CreateClient();
        JsonDocument response = await MakeRequestAsync(client, $"/repos/{organization}/{repo}");

        return response.Deserialize<GitHubModels.Repo>() ?? throw new InvalidDataException($"Request failed: {nameof(GetRepositoryAsync)}");
    }

    [KernelFunction]
    [Description("Get issues for a repository")]
    public async Task<GitHubModels.Issue[]> GetIssuesAsync(
        string organization,
        string repo,
        [Description("default count is 30")]
        int? maxResults = null,
        [Description("open, closed, or all")]
        string state = "",
        string label = "",
        string assignee = "")
    {
        using HttpClient client = this.CreateClient();

        string path = $"/repos/{organization}/{repo}/issues?";
        path = BuildQuery(path, "state", state);
        path = BuildQuery(path, "assignee", assignee);
        path = BuildQuery(path, "labels", label);
        path = BuildQuery(path, "per_page", maxResults?.ToString() ?? string.Empty);

        JsonDocument response = await MakeRequestAsync(client, path);

        return response.Deserialize<GitHubModels.Issue[]>() ?? throw new InvalidDataException($"Request failed: {nameof(GetIssuesAsync)}");
    }

    [KernelFunction]
    [Description("Get details for a specific issue")]
    public async Task<GitHubModels.IssueDetail> GetIssueDetailAsync(string organization, string repo, int issueId)
    {
        using HttpClient client = this.CreateClient();

        string path = $"/repos/{organization}/{repo}/issues/{issueId}";

        JsonDocument response = await MakeRequestAsync(client, path);

        return response.Deserialize<GitHubModels.IssueDetail>() ?? throw new InvalidDataException($"Request failed: {nameof(GetIssueDetailAsync)}");
    }

    [KernelFunction]
    public async Task<GitHubModels.Commit> GetLatestCommitAsync(string organization, string repo, string branch = "main")
    {
        using HttpClient client = this.CreateClient();

        string path = $"/repos/{organization}/{repo}/commits/{branch}";

        JsonDocument response = await MakeRequestAsync(client, path);

        return response.Deserialize<GitHubModels.Commit>() ?? throw new InvalidDataException($"Request failed: {nameof(GetLatestCommitAsync)}");
    }

    [KernelFunction]
    public async Task<bool> TagExistsAsync(string organization, string repo, string tagName)
    {
        try
        {
            using HttpClient client = this.CreateClient();
            string path = $"/repos/{organization}/{repo}/git/refs/tags/{tagName}";
            HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative));
            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    [KernelFunction]
    [Description("Verifies that a tag was successfully created and is visible")]
    public async Task<JsonDocument?> VerifyTagCreatedAsync(string organization, string repo, string tagName)
    {
        try
        {
            using HttpClient client = this.CreateClient();
            string path = $"/repos/{organization}/{repo}/git/refs/tags/{tagName}";
            Console.WriteLine($"🔍 Verifying tag exists: {path}");

            HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative));
            if (response.IsSuccessStatusCode)
            {
                string content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"✅ Tag verified: {tagName}");
                return JsonDocument.Parse(content);
            }
            else
            {
                Console.WriteLine($"❌ Tag not found: {tagName} (Status: {response.StatusCode})");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error verifying tag: {ex.Message}");
            return null;
        }
    }

    [KernelFunction]
    public async Task<GitHubModels.CreateTagResponse> CreateTagAsync(
        string organization,
        string repo,
        string tagName,
        [Description("Optional message for the tag. If not provided, uses default message.")]
        string message = "Tagged by Release Agent",
        [Description("Optional branch name. Defaults to 'main'.")]
        string branch = "main")
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(organization))
            throw new ArgumentException("Organization cannot be null or empty", nameof(organization));
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty", nameof(repo));
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        // Validate tag name format (GitHub requirements)
        if (tagName.Contains(" ") || tagName.StartsWith(".") || tagName.EndsWith(".") ||
            tagName.Contains("..") || tagName.Contains("~") || tagName.Contains("^") ||
            tagName.Contains(":") || tagName.Contains("?") || tagName.Contains("*") ||
            tagName.Contains("[") || tagName.Contains("\\"))
        {
            throw new ArgumentException("Tag name contains invalid characters. Tag names cannot contain spaces, start/end with dots, or contain special characters like ~^:?*[\\", nameof(tagName));
        }

        // Check if tag already exists
        if (await TagExistsAsync(organization, repo, tagName))
        {
            throw new InvalidOperationException($"Tag '{tagName}' already exists in repository {organization}/{repo}");
        }

        // Get authenticated user info for tagger
        var user = await GetUserProfileAsync();

        // First get the latest commit
        var latestCommit = await GetLatestCommitAsync(organization, repo, branch);

        // Ensure we have a full SHA
        if (string.IsNullOrWhiteSpace(latestCommit.Sha) || latestCommit.Sha.Length != 40)
        {
            throw new InvalidDataException($"Invalid commit SHA received: {latestCommit.Sha}");
        }

        using HttpClient client = this.CreateClient();

        // Use default message if not provided
        if (string.IsNullOrWhiteSpace(message))
        {
            message = $"Release {tagName}";
        }

        // Try first without tagger (simpler approach)
        var createTagRequest = new GitHubModels.CreateTagRequest
        {
            TagName = tagName,
            Message = message,
            ObjectSha = latestCommit.Sha,
            Type = "commit"
            // Tagger is optional - GitHub will use the authenticated user
        };

        // Step 1: Create the tag object
        string tagPath = $"/repos/{organization}/{repo}/git/tags";
        JsonDocument tagResponse = await MakePostRequestAsync(client, tagPath, createTagRequest);

        var tagResult = tagResponse.Deserialize<GitHubModels.CreateTagResponse>();
        if (tagResult == null)
        {
            throw new InvalidDataException("Failed to create tag object");
        }

        Console.WriteLine($"✅ Tag object created with SHA: {tagResult.Sha}");

        // Step 2: Create the reference to make the tag visible
        var createRefRequest = new
        {
            @ref = $"refs/tags/{tagName}",
            sha = tagResult.Sha
        };

        string refPath = $"/repos/{organization}/{repo}/git/refs";
        JsonDocument refResponse = await MakePostRequestAsync(client, refPath, createRefRequest);

        Console.WriteLine($"✅ Tag reference created: refs/tags/{tagName}");

        // Step 3: Verify the tag was created and is visible
        await Task.Delay(1000); // Small delay to ensure consistency
        var verification = await VerifyTagCreatedAsync(organization, repo, tagName);
        if (verification == null)
        {
            throw new InvalidOperationException($"Tag '{tagName}' was created but verification failed. It may take a moment to appear.");
        }

        return tagResult;
    }

    [KernelFunction]
    [Description("Creates a lightweight tag reference (alternative to annotated tag)")]
    public async Task<JsonDocument> CreateLightweightTagAsync(
        string organization,
        string repo,
        string tagName,
        [Description("Optional branch name. Defaults to 'main'.")]
        string branch = "main")
    {
        // Validate inputs
        if (string.IsNullOrWhiteSpace(organization))
            throw new ArgumentException("Organization cannot be null or empty", nameof(organization));
        if (string.IsNullOrWhiteSpace(repo))
            throw new ArgumentException("Repository cannot be null or empty", nameof(repo));
        if (string.IsNullOrWhiteSpace(tagName))
            throw new ArgumentException("Tag name cannot be null or empty", nameof(tagName));

        // Check if tag already exists
        if (await TagExistsAsync(organization, repo, tagName))
        {
            throw new InvalidOperationException($"Tag '{tagName}' already exists in repository {organization}/{repo}");
        }

        // Get the latest commit
        var latestCommit = await GetLatestCommitAsync(organization, repo, branch);

        using HttpClient client = this.CreateClient();

        // Create reference request for lightweight tag
        var createRefRequest = new
        {
            @ref = $"refs/tags/{tagName}",
            sha = latestCommit.Sha
        };

        string path = $"/repos/{organization}/{repo}/git/refs";

        JsonDocument response = await MakePostRequestAsync(client, path, createRefRequest);

        Console.WriteLine($"✅ Lightweight tag created: {tagName}");

        // Verify the tag was created
        await Task.Delay(500); // Small delay to ensure consistency
        var verification = await VerifyTagCreatedAsync(organization, repo, tagName);
        if (verification == null)
        {
            throw new InvalidOperationException($"Lightweight tag '{tagName}' was created but verification failed. It may take a moment to appear.");
        }

        return response;
    }

    [KernelFunction]
    [Description("List all tags in the repository")]
    public async Task<GitHubModels.Tag[]> ListTagsAsync(string organization, string repo)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/tags";
        Console.WriteLine($"📋 Listing all tags for {organization}/{repo}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.Tag[]>() ?? throw new InvalidDataException($"Request failed: {nameof(ListTagsAsync)}");
    }

    [KernelFunction]
    [Description("Compare commits between two references (tags, branches, or SHAs)")]
    public async Task<GitHubModels.CompareResult> CompareCommitsAsync(string organization, string repo, string baseRef, string headRef)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/compare/{baseRef}...{headRef}";
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.CompareResult>() ?? throw new InvalidDataException($"Request failed: {nameof(CompareCommitsAsync)}");
    }

    [KernelFunction]
    [Description("Get details for a specific commit by its SHA")]
    public async Task<GitHubModels.CommitDetail> GetCommitDetailAsync(string organization, string repo, string commitSha)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/commits/{commitSha}";
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.CommitDetail>() ?? throw new InvalidDataException($"Request failed: {nameof(GetCommitDetailAsync)}");
    }

    [KernelFunction]
    [Description("Generate a structured change log between two tags")]
    public async Task<GitHubModels.ChangeLog> GenerateChangeLogAsync(
        string organization,
        string repo,
        [Description("The earlier/base tag (e.g., 'v1.0.0'). If null, uses the second-to-last tag.")]
        string? fromTag = null,
        [Description("The later/head tag (e.g., 'v1.1.0'). If null, uses the latest tag.")]
        string? toTag = null)
    {
        // Get all tags if we need to determine fromTag or toTag
        var tags = await ListTagsAsync(organization, repo);
        if (tags.Length == 0)
        {
            throw new InvalidOperationException("No tags found in repository");
        }

        // Determine the tags to compare
        if (string.IsNullOrEmpty(toTag))
        {
            toTag = tags[0].Name; // Latest tag
        }

        if (string.IsNullOrEmpty(fromTag))
        {
            if (tags.Length < 2)
            {
                throw new InvalidOperationException("Need at least 2 tags to generate changelog, or specify fromTag");
            }
            fromTag = tags[1].Name; // Second-to-last tag
        }

        Console.WriteLine($"📊 Generating changelog from {fromTag} to {toTag}");

        // Compare commits between the tags
        var comparison = await CompareCommitsAsync(organization, repo, fromTag, toTag);

        // Generate change log entries
        var entries = new List<GitHubModels.ChangeLogEntry>();
        var contributors = new HashSet<string>();

        foreach (var commit in comparison.Commits)
        {
            var entry = await CreateChangeLogEntry(organization, repo, commit);
            entries.Add(entry);

            if (!string.IsNullOrEmpty(entry.Author))
            {
                contributors.Add(entry.Author);
            }
        }

        // Calculate total stats
        var totalStats = new GitHubModels.CommitStats
        {
            Total = comparison.Files?.Sum(f => f.Changes) ?? 0,
            Additions = comparison.Files?.Sum(f => f.Additions) ?? 0,
            Deletions = comparison.Files?.Sum(f => f.Deletions) ?? 0
        };

        var changeLog = new GitHubModels.ChangeLog
        {
            FromTag = fromTag,
            ToTag = toTag,
            GeneratedAt = DateTime.UtcNow,
            TotalCommits = comparison.TotalCommits,
            TotalStats = totalStats,
            Entries = entries.ToArray(),
            Contributors = contributors.ToArray()
        };

        Console.WriteLine($"✅ Generated changelog with {changeLog.TotalCommits} commits from {contributors.Count} contributors");

        return changeLog;
    }

    [KernelFunction]
    [Description("Generate a formatted markdown change log between two tags")]
    public async Task<string> GenerateMarkdownChangeLogAsync(
        string organization,
        string repo,
        string? fromTag = null,
        string? toTag = null)
    {
        var changeLog = await GenerateChangeLogAsync(organization, repo, fromTag, toTag);
        return FormatChangeLogAsMarkdown(changeLog);
    }

    [KernelFunction]
    [Description("Generate release notes and commit them directly to the GitHub repository in ReleaseNotes folder")]
    public async Task<string> GenerateAndSaveChangeLogAsync(
        string organization,
        string repo,
        [Description("The earlier/base tag (e.g., 'v1.0.0'). If null, uses the second-to-last tag.")]
        string? fromTag = null,
        [Description("The later/head tag (e.g., 'v1.1.0'). If null, uses the latest tag.")]
        string? toTag = null,
        [Description("Branch to commit to. Defaults to 'main'.")]
        string branch = "main")
    {
        // Generate the markdown changelog
        var markdownContent = await GenerateMarkdownChangeLogAsync(organization, repo, fromTag, toTag);

        // Create filename with timestamp
        var fileName = $"changelog_{DateTime.Now:yyyyMMdd_HHmmss}.md";
        var filePath = $"ReleaseNotes/{fileName}";

        Console.WriteLine($"📝 Creating changelog in repository: {organization}/{repo}");
        Console.WriteLine($"📄 File path: {filePath}");
        Console.WriteLine($"📄 Content size: {markdownContent.Length:N0} characters");

        // Create/update the file in the GitHub repository
        var result = await CreateFileInRepositoryAsync(organization, repo, filePath, markdownContent, fromTag, toTag, branch);

        var message = $"✅ Release notes committed to {organization}/{repo} at {filePath}";
        Console.WriteLine(message);
        Console.WriteLine($"🔗 Commit URL: {result.Commit?.HtmlUrl}");

        return message;
    }

    private async Task<GitHubModels.CreateFileResponse> CreateFileInRepositoryAsync(
        string organization,
        string repo,
        string filePath,
        string content,
        string? fromTag,
        string? toTag,
        string branch)
    {
        using HttpClient client = this.CreateClient();

        // Get authenticated user for committer info
        var user = await GetUserProfileAsync();

        // Create commit message
        var commitMessage = fromTag != null && toTag != null
            ? $"docs: add release notes for {fromTag} to {toTag}"
            : "docs: add release notes";

        // Encode content to Base64
        var base64Content = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(content));

        var createFileRequest = new GitHubModels.CreateFileRequest
        {
            Message = commitMessage,
            Content = base64Content,
            Branch = branch,
            Committer = new GitHubModels.GitCommitter
            {
                Name = user.Name ?? user.Login,
                Email = "noreply@github.com"
            }
        };

        string path = $"/repos/{organization}/{repo}/contents/{filePath}";
        JsonDocument response = await MakePutRequestAsync(client, path, createFileRequest);
        //JsonDocument response = await MakePostRequestAsync(client, path, createFileRequest);

        return response.Deserialize<GitHubModels.CreateFileResponse>() ??
               throw new InvalidDataException($"Request failed: {nameof(CreateFileInRepositoryAsync)}");
    }


    private async Task<GitHubModels.ChangeLogEntry> CreateChangeLogEntry(string organization, string repo, GitHubModels.CommitDetail commit)
    {
        // Get detailed commit info
        var commitDetail = await GetCommitDetailAsync(organization, repo, commit.Sha);

        // Parse commit type from conventional commit format
        string commitType = ParseCommitType(commit.Commit?.Message ?? "");

        // Get file changes
        var fileChanges = commitDetail.Files?.Select(f => $"{f.Status}: {f.FileName}").ToArray() ?? Array.Empty<string>();

        // Parse date
        var commitDate = DateTime.TryParse(commit.Commit?.Author?.Date, out var parsedDate)
            ? parsedDate
            : DateTime.UtcNow;

        return new GitHubModels.ChangeLogEntry
        {
            Sha = commit.Sha,
            Message = commit.Commit?.Message ?? "",
            Author = commit.Author?.Login ?? commit.Commit?.Author?.Name ?? "Unknown",
            Date = commitDate,
            Type = commitType,
            FileChanges = fileChanges,
            Stats = commitDetail.Stats ?? new GitHubModels.CommitStats()
        };
    }

    private static string ParseCommitType(string message)
    {
        if (string.IsNullOrEmpty(message))
            return "other";

        // Parse conventional commit format: type(scope): description
        var match = Regex.Match(message, @"^(\w+)(?:\([^)]+\))?:", RegexOptions.IgnoreCase);
        if (match.Success)
        {
            return match.Groups[1].Value.ToLowerInvariant();
        }

        // Fallback: detect common patterns
        var lowerMessage = message.ToLowerInvariant();
        if (lowerMessage.StartsWith("fix") || lowerMessage.Contains("bug"))
            return "fix";
        if (lowerMessage.StartsWith("feat") || lowerMessage.StartsWith("add"))
            return "feat";
        if (lowerMessage.StartsWith("docs") || lowerMessage.Contains("documentation"))
            return "docs";
        if (lowerMessage.StartsWith("refactor"))
            return "refactor";
        if (lowerMessage.StartsWith("test"))
            return "test";
        if (lowerMessage.StartsWith("chore"))
            return "chore";

        return "other";
    }

    private static string FormatChangeLogAsMarkdown(GitHubModels.ChangeLog changeLog)
    {
        var sb = new StringBuilder();

        // Header
        sb.AppendLine($"# Release Notes: {changeLog.FromTag} → {changeLog.ToTag}");
        sb.AppendLine();
        sb.AppendLine($"**Generated:** {changeLog.GeneratedAt:yyyy-MM-dd HH:mm:ss} UTC");
        sb.AppendLine($"**Total Commits:** {changeLog.TotalCommits}");
        sb.AppendLine($"**Contributors:** {changeLog.Contributors.Length}");
        sb.AppendLine($"**Files Changed:** {changeLog.TotalStats.Total}");
        sb.AppendLine($"**Lines Added:** +{changeLog.TotalStats.Additions}");
        sb.AppendLine($"**Lines Removed:** -{changeLog.TotalStats.Deletions}");
        sb.AppendLine();

        // Group entries by type
        var groupedEntries = changeLog.Entries
            .GroupBy(e => e.Type)
            .OrderBy(g => GetTypeOrder(g.Key));

        foreach (var group in groupedEntries)
        {
            sb.AppendLine($"## {GetTypeTitle(group.Key)}");
            sb.AppendLine();

            foreach (var entry in group.OrderByDescending(e => e.Date))
            {
                var shortSha = entry.Sha[..Math.Min(7, entry.Sha.Length)];
                var firstLine = entry.Message.Split('\n')[0];
                var statsText = $"(+{entry.Stats.Additions}/-{entry.Stats.Deletions})";

                sb.AppendLine($"- **{firstLine}** [`{shortSha}`] by @{entry.Author} {statsText}");

                if (entry.FileChanges.Length > 0 && entry.FileChanges.Length <= 5)
                {
                    foreach (var file in entry.FileChanges)
                    {
                        sb.AppendLine($"  - {file}");
                    }
                }
                else if (entry.FileChanges.Length > 5)
                {
                    sb.AppendLine($"  - *{entry.FileChanges.Length} files changed*");
                }

                sb.AppendLine();
            }
        }

        // Contributors section
        if (changeLog.Contributors.Length > 0)
        {
            sb.AppendLine("## Contributors");
            sb.AppendLine();
            foreach (var contributor in changeLog.Contributors.OrderBy(c => c))
            {
                sb.AppendLine($"- @{contributor}");
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    private static int GetTypeOrder(string type)
    {
        return type switch
        {
            "feat" => 1,
            "fix" => 2,
            "perf" => 3,
            "refactor" => 4,
            "docs" => 5,
            "test" => 6,
            "chore" => 7,
            _ => 8
        };
    }

    private static string GetTypeTitle(string type)
    {
        return type switch
        {
            "feat" => "🚀 Features",
            "fix" => "🐛 Bug Fixes",
            "perf" => "⚡ Performance",
            "refactor" => "♻️ Refactoring",
            "docs" => "📚 Documentation",
            "test" => "🧪 Tests",
            "chore" => "🔧 Chores",
            _ => "📝 Other Changes"
        };
    }

    private HttpClient CreateClient()
    {
       Console.WriteLine("🔐 Creating GitHub HTTP client");
        HttpClient client = new()
        {
            BaseAddress = new Uri(settings.GitSettings.BaseUrl)
        };

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "request");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {settings.GitSettings.Token}");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        return client;
    }

    private static string BuildQuery(string path, string key, string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            return $"{path}{key}={value}&";
        }

        return path;
    }

    private static async Task<JsonDocument> MakeRequestAsync(HttpClient client, string path)
    {
        Console.WriteLine($"REQUEST: {path}");
        Console.WriteLine();

        HttpResponseMessage response = await client.GetAsync(new Uri(path, UriKind.Relative));
        response.EnsureSuccessStatusCode();
        string content = await response.Content.ReadAsStringAsync();
        return JsonDocument.Parse(content);
    }

    private static async Task<JsonDocument> MakePostRequestAsync<T>(HttpClient client, string path, T requestBody)
    {
        Console.WriteLine($"POST REQUEST: {path}");
        Console.WriteLine();

        string jsonContent = JsonSerializer.Serialize(requestBody);
        Console.WriteLine("REQUEST BODY:");
        Console.WriteLine(jsonContent);
        Console.WriteLine();

        StringContent content = new(jsonContent, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PostAsync(new Uri(path, UriKind.Relative), content);

        string responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"RESPONSE STATUS: {response.StatusCode}");
        Console.WriteLine("RESPONSE BODY:");
        Console.WriteLine(responseContent);
        Console.WriteLine();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {responseContent}");
        }

        return JsonDocument.Parse(responseContent);
    }

    private static async Task<JsonDocument> MakePutRequestAsync<T>(HttpClient client, string path, T requestBody)
    {
        Console.WriteLine($"PUT REQUEST: {path}");
        Console.WriteLine();

        string jsonContent = JsonSerializer.Serialize(requestBody);
        Console.WriteLine("REQUEST BODY:");
        Console.WriteLine(jsonContent);
        Console.WriteLine();

        StringContent content = new(jsonContent, System.Text.Encoding.UTF8, "application/json");

        HttpResponseMessage response = await client.PutAsync(new Uri(path, UriKind.Relative), content);

        string responseContent = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"RESPONSE STATUS: {response.StatusCode}");
        Console.WriteLine("RESPONSE BODY:");
        Console.WriteLine(responseContent);
        Console.WriteLine();

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException($"Request failed with status {response.StatusCode}: {responseContent}");
        }

        return JsonDocument.Parse(responseContent);
    }


}
