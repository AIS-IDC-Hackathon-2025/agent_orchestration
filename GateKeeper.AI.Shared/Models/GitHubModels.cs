using System.Text.Json.Serialization;

namespace GateKeeper.AI.Shared.Models;

public static class GitHubModels
{
    public sealed class Repo
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("full_name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }
    }

    public sealed class User
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("company")]
        public string Company { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }
    }

    public class Issue
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("labels")]
        public Label[] Labels { get; set; }

        [JsonPropertyName("created_at")]
        public string WhenCreated { get; set; }

        [JsonPropertyName("closed_at")]
        public string WhenClosed { get; set; }
    }

    public sealed class IssueDetail : Issue
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public sealed class Label
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

    public sealed class Commit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("commit")]
        public CommitDetails CommitDetails { get; set; }
    }

    public sealed class CommitDetails
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("author")]
        public CommitAuthor Author { get; set; }

        [JsonPropertyName("committer")]
        public CommitAuthor Committer { get; set; }
    }

    public sealed class CommitAuthor
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }

    public sealed class Tag
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("zipball_url")]
        public string ZipballUrl { get; set; }

        [JsonPropertyName("tarball_url")]
        public string TarballUrl { get; set; }

        [JsonPropertyName("commit")]
        public TagCommit Commit { get; set; }
    }

    public sealed class TagCommit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public sealed class CreateTagRequest
    {
        [JsonPropertyName("tag")]
        public string TagName { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("object")]
        public string ObjectSha { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = "commit";

        [JsonPropertyName("tagger")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public TagTagger? Tagger { get; set; }
    }

    public sealed class TagTagger
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }

    public sealed class CreateTagResponse
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("tag")]
        public string Tag { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("object")]
        public TagObject Object { get; set; }
    }

    public sealed class TagObject
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    // Additional models for changelog generation
    public sealed class CommitDetail
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("commit")]
        public CommitContent Commit { get; set; }

        [JsonPropertyName("author")]
        public User Author { get; set; }

        [JsonPropertyName("committer")]
        public User Committer { get; set; }

        [JsonPropertyName("parents")]
        public ParentRef[] Parents { get; set; }

        [JsonPropertyName("files")]
        public CommitFile[] Files { get; set; }

        [JsonPropertyName("stats")]
        public CommitStats Stats { get; set; }
    }

    public sealed class CommitContent
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("author")]
        public GitSignature Author { get; set; }

        [JsonPropertyName("committer")]
        public GitSignature Committer { get; set; }
    }

    public sealed class GitSignature
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("date")]
        public string Date { get; set; }
    }

    public sealed class ParentRef
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }
    }

    public sealed class CommitFile
    {
        [JsonPropertyName("filename")]
        public string FileName { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("additions")]
        public int Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }

        [JsonPropertyName("changes")]
        public int Changes { get; set; }

        [JsonPropertyName("patch")]
        public string Patch { get; set; }
    }

    public sealed class CommitStats
    {
        [JsonPropertyName("total")]
        public int Total { get; set; }

        [JsonPropertyName("additions")]
        public int Additions { get; set; }

        [JsonPropertyName("deletions")]
        public int Deletions { get; set; }
    }

    public sealed class CompareResult
    {
        [JsonPropertyName("html_url")]
        public string Url { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("ahead_by")]
        public int AheadBy { get; set; }

        [JsonPropertyName("behind_by")]
        public int BehindBy { get; set; }

        [JsonPropertyName("total_commits")]
        public int TotalCommits { get; set; }

        [JsonPropertyName("commits")]
        public CommitDetail[] Commits { get; set; }

        [JsonPropertyName("files")]
        public CommitFile[] Files { get; set; }
    }

    // Change log entry for formatted output
    public sealed class ChangeLogEntry
    {
        public string Sha { get; set; }
        public string Message { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; } // feat, fix, docs, etc.
        public string[] FileChanges { get; set; }
        public CommitStats Stats { get; set; }
    }

    // Full change log between two tags
    public sealed class ChangeLog
    {
        public string FromTag { get; set; }
        public string ToTag { get; set; }
        public DateTime GeneratedAt { get; set; }
        public int TotalCommits { get; set; }
        public CommitStats TotalStats { get; set; }
        public ChangeLogEntry[] Entries { get; set; }
        public string[] Contributors { get; set; }
    }

    // Models for GitHub Contents API (file creation)
    public sealed class CreateFileRequest
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; } // Base64 encoded content

        [JsonPropertyName("branch")]
        public string Branch { get; set; }

        [JsonPropertyName("committer")]
        public GitCommitter Committer { get; set; }
    }

    public sealed class GitCommitter
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }
    }

    public sealed class CreateFileResponse
    {
        [JsonPropertyName("content")]
        public FileContent Content { get; set; }

        [JsonPropertyName("commit")]
        public FileCommit Commit { get; set; }
    }

    public sealed class FileContent
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("size")]
        public int Size { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("download_url")]
        public string DownloadUrl { get; set; }
    }

    public sealed class FileCommit
    {
        [JsonPropertyName("sha")]
        public string Sha { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("author")]
        public GitSignature Author { get; set; }

        [JsonPropertyName("committer")]
        public GitSignature Committer { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class DependabotAlert
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("dependency")]
        public DependabotDependency Dependency { get; set; }

        [JsonPropertyName("security_advisory")]
        public DependabotSecurityAdvisory SecurityAdvisory { get; set; }

        [JsonPropertyName("security_vulnerability")]
        public DependabotSecurityVulnerability SecurityVulnerability { get; set; }

        [JsonPropertyName("dismissed_at")]
        public string DismissedAt { get; set; }

        [JsonPropertyName("dismissed_by")]
        public User DismissedBy { get; set; }

        [JsonPropertyName("dismissed_reason")]
        public string DismissedReason { get; set; }

        [JsonPropertyName("html_url")]
        public string Url { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("fixed_at")]
        public string FixedAt { get; set; }

        [JsonPropertyName("auto_dismissed_at")]
        public string AutoDismissedAt { get; set; }

        [JsonPropertyName("manifest_path")]
        public string ManifestPath { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public sealed class DependabotDependency
    {
        [JsonPropertyName("package")]
        public DependabotPackage Package { get; set; }

        [JsonPropertyName("manifest_path")]
        public string ManifestPath { get; set; }

        [JsonPropertyName("scope")]
        public string Scope { get; set; }
    }

    public sealed class DependabotPackage
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("ecosystem")]
        public string Ecosystem { get; set; }
    }

    public sealed class DependabotSecurityAdvisory
    {
        [JsonPropertyName("ghsa_id")]
        public string GhsaId { get; set; }

        [JsonPropertyName("summary")]
        public string Summary { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        [JsonPropertyName("cve_id")]
        public string CveId { get; set; }

        [JsonPropertyName("references")]
        public DependabotReference[] References { get; set; }
    }

    public sealed class DependabotReference
    {
        [JsonPropertyName("url")]
        public string Url { get; set; }
    }

    public sealed class DependabotSecurityVulnerability
    {
        [JsonPropertyName("package")]
        public DependabotPackage Package { get; set; }

        [JsonPropertyName("vulnerable_version_range")]
        public string VulnerableVersionRange { get; set; }

        [JsonPropertyName("first_patched_version")]
        public DependabotPatchedVersion FirstPatchedVersion { get; set; }

        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }

    public sealed class DependabotPatchedVersion
    {
        [JsonPropertyName("identifier")]
        public string Identifier { get; set; }
    }

    // Code Scanning models (GitHub Code Scanning Alert API)
    public sealed class CodeScanningAlert
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("dismissed_at")]
        public string DismissedAt { get; set; }

        [JsonPropertyName("dismissed_by")]
        public User DismissedBy { get; set; }

        [JsonPropertyName("dismissed_reason")]
        public string DismissedReason { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("instances_url")]
        public string InstancesUrl { get; set; }

        [JsonPropertyName("rule")]
        public CodeScanningRule Rule { get; set; }

        [JsonPropertyName("tool")]
        public CodeScanningTool Tool { get; set; }

        [JsonPropertyName("most_recent_instance")]
        public CodeScanningAlertInstance MostRecentInstance { get; set; }
    }

    public sealed class CodeScanningRule
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        // e.g. "warning", "error", "note"
        [JsonPropertyName("severity")]
        public string Severity { get; set; }

        // e.g. "low", "medium", "high", "critical"
        [JsonPropertyName("security_severity_level")]
        public string SecuritySeverityLevel { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("full_description")]
        public string FullDescription { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; }
    }

    public sealed class CodeScanningTool
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("guid")]
        public string Guid { get; set; }
    }

    public sealed class CodeScanningAlertInstance
    {
        [JsonPropertyName("ref")]
        public string Ref { get; set; }

        [JsonPropertyName("analysis_key")]
        public string AnalysisKey { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("classifications")]
        public string[] Classifications { get; set; }

        [JsonPropertyName("message")]
        public CodeScanningMessage Message { get; set; }

        [JsonPropertyName("location")]
        public CodeScanningAlertLocation Location { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("fixed_at")]
        public string FixedAt { get; set; }
    }

    public sealed class CodeScanningMessage
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }
    }

    public sealed class CodeScanningAlertLocation
    {
        [JsonPropertyName("path")]
        public string Path { get; set; }

        [JsonPropertyName("start_line")]
        public int? StartLine { get; set; }

        [JsonPropertyName("end_line")]
        public int? EndLine { get; set; }

        [JsonPropertyName("start_column")]
        public int? StartColumn { get; set; }

        [JsonPropertyName("end_column")]
        public int? EndColumn { get; set; }
    }

    // Secret Scanning Alert models (GitHub Secret Scanning API)
    public sealed class SecretScanningAlert
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("secret_type")]
        public string SecretType { get; set; }

        [JsonPropertyName("secret_type_display_name")]
        public string SecretTypeDisplayName { get; set; }

        [JsonPropertyName("secret")]
        public string Secret { get; set; }

        [JsonPropertyName("resolution")]
        public string Resolution { get; set; }

        [JsonPropertyName("resolved_at")]
        public string ResolvedAt { get; set; }

        [JsonPropertyName("resolved_by")]
        public User ResolvedBy { get; set; }

        [JsonPropertyName("push_protection_bypassed")]
        public bool? PushProtectionBypassed { get; set; }

        [JsonPropertyName("push_protection_bypassed_by")]
        public User PushProtectionBypassedBy { get; set; }

        [JsonPropertyName("push_protection_bypassed_at")]
        public string PushProtectionBypassedAt { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("locations_url")]
        public string LocationsUrl { get; set; }

        [JsonPropertyName("created_at")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updated_at")]
        public string UpdatedAt { get; set; }
    }
}
