using GateKeeper.AI.Shared.Models;
using Microsoft.SemanticKernel;
using System.ComponentModel;
using System.Text.Json;
using static GateKeeper.AI.Shared.Settings;

namespace GateKeeper.AI.Trust.Agent.Plugins;

public class TrustAgentGitHubPlugin
{
    private readonly GitHubSettings _gitHubSettings;

    public TrustAgentGitHubPlugin(GitHubSettings gitHubSettings)
    {
        _gitHubSettings = gitHubSettings;
    }

    //// Common CodeQL workflow filename candidates (with and without full path)
    //private static readonly string[] s_codeQlWorkflowCandidates =
    //[
    //    "codeql.yml",
    //    "codeql.yaml",
    //    "codeql-analysis.yml",
    //    "codeql-analysis.yaml",
    //    ".github/workflows/codeql.yml",
    //    ".github/workflows/codeql.yaml",
    //    ".github/workflows/codeql-analysis.yml",
    //    ".github/workflows/codeql-analysis.yaml"
    //];

    // 1. Dependency & Vulnerability Alerts
    // GET /repos/{owner}/{repo}/dependabot/alerts
    // Docs: https://docs.github.com/en/rest/dependabot/alerts#list-dependabot-alerts-for-a-repository
    [KernelFunction]
    [Description("List Dependabot alerts for a repository")]
    public async Task<GitHubModels.DependabotAlert[]> ListDependabotAlertsAsync(string organization, string repo, string state = "open")
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/dependabot/alerts?state={state}";
        Console.WriteLine($"📋 Listing Dependabot alerts for {organization}/{repo} with state={state}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.DependabotAlert[]>() ?? throw new InvalidDataException($"Request failed: {nameof(ListDependabotAlertsAsync)}");
    }

    // 2. Get Dependabot Alert
    // GET /repos/{owner}/{repo}/dependabot/alerts/{alert_number}
    // Docs: https://docs.github.com/en/rest/dependabot/alerts#get-a-dependabot-alert
    [KernelFunction]
    [Description("Get a specific Dependabot alert by its number")]
    public async Task<GitHubModels.DependabotAlert> GetDependabotAlertAsync(string organization, string repo, int alertNumber)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/dependabot/alerts/{alertNumber}";
        Console.WriteLine($"📋 Getting Dependabot alert #{alertNumber} for {organization}/{repo}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.DependabotAlert>() ?? throw new InvalidDataException($"Request failed: {nameof(GetDependabotAlertAsync)}");
    }

    // 3. Code Scanning Alerts
    // List Code Scanning Alerts
    // GET /repos/{owner}/{repo}/code-scanning/alerts
    // Docs: https://docs.github.com/en/rest/code-scanning#list-code-scanning-alerts-for-a-repository
    [KernelFunction]
    [Description("List Code Scanning alerts for a repository")]
    public async Task<GitHubModels.CodeScanningAlert[]> ListCodeScanningAlertsAsync(string organization, string repo, string state = "open")
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/code-scanning/alerts?state={state}";
        Console.WriteLine($"📋 Listing Code Scanning alerts for {organization}/{repo} with state={state}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.CodeScanningAlert[]>() ?? throw new InvalidDataException($"Request failed: {nameof(ListCodeScanningAlertsAsync)}");
    }

    // 4. Get Code Scanning Alert
    // GET /repos/{owner}/{repo}/code-scanning/alerts/{alert_number}
    // Docs: https://docs.github.com/en/rest/code-scanning#get-a-code-scanning-alert
    [KernelFunction]
    [Description("Get a specific Code Scanning alert by its number")]
    public async Task<GitHubModels.CodeScanningAlert> GetCodeScanningAlertAsync(string organization, string repo, int alertNumber)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/code-scanning/alerts/{alertNumber}";
        Console.WriteLine($"📋 Getting Code Scanning alert #{alertNumber} for {organization}/{repo}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.CodeScanningAlert>() ?? throw new InvalidDataException($"Request failed: {nameof(GetCodeScanningAlertAsync)}");
    }

    // 5. Secret Scanning Alerts
    // List Secret Scanning Alerts
    // GET /repos/{owner}/{repo}/secret-scanning/alerts
    // Docs: https://docs.github.com/en/rest/secret-scanning#list-secret-scanning-alerts-for-a-repository
    [KernelFunction]
    [Description("List Secret Scanning alerts for a repository")]
    public async Task<GitHubModels.SecretScanningAlert[]> ListSecretScanningAlertsAsync(string organization, string repo, string state = "open")
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/secret-scanning/alerts?state={state}";
        Console.WriteLine($"📋 Listing Secret Scanning alerts for {organization}/{repo} with state={state}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.SecretScanningAlert[]>() ?? throw new InvalidDataException($"Request failed: {nameof(ListSecretScanningAlertsAsync)}");
    }

    // 6. Get Secret Scanning Alert
    // GET /repos/{owner}/{repo}/secret-scanning/alerts/{alert_number}
    // Docs: https://docs.github.com/en/rest/secret-scanning#get-a-secret-scanning-alert
    [KernelFunction]
    [Description("Get a specific Secret Scanning alert by its number")]
    public async Task<GitHubModels.SecretScanningAlert> GetSecretScanningAlertAsync(string organization, string repo, int alertNumber)
    {
        using HttpClient client = this.CreateClient();
        string path = $"/repos/{organization}/{repo}/secret-scanning/alerts/{alertNumber}";
        Console.WriteLine($"📋 Getting Secret Scanning alert #{alertNumber} for {organization}/{repo}");
        JsonDocument response = await MakeRequestAsync(client, path);
        return response.Deserialize<GitHubModels.SecretScanningAlert>() ?? throw new InvalidDataException($"Request failed: {nameof(GetSecretScanningAlertAsync)}");
    }

    #region Common

    private HttpClient CreateClient()
    {
        HttpClient client = new()
        {
            BaseAddress = new Uri(_gitHubSettings.BaseUrl)
        };

        client.DefaultRequestHeaders.Clear();
        client.DefaultRequestHeaders.Add("User-Agent", "request");
        client.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {_gitHubSettings.Token}");
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        return client;
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

    #endregion
}