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

    #region Trust
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

    // License models (GitHub License API)
    public sealed class License
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("spdx_id")]
        public string SpdxId { get; set; }

        [JsonPropertyName("url")]
        public string Url { get; set; }

        [JsonPropertyName("node_id")]
        public string NodeId { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("implementation")]
        public string Implementation { get; set; }

        [JsonPropertyName("permissions")]
        public string[] Permissions { get; set; }

        [JsonPropertyName("conditions")]
        public string[] Conditions { get; set; }

        [JsonPropertyName("limitations")]
        public string[] Limitations { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("featured")]
        public bool Featured { get; set; }
    }

    // Dependency Graph comparison models (GitHub: dependency-graph/compare)
    // Endpoint: GET /repos/{owner}/{repo}/dependency-graph/compare/{base}...{head}
    // These classes intentionally model only documented/public fields and allow
    // forward compatibility via nullable/optional properties.
    public sealed class DependencyComparison
    {
        [JsonPropertyName("base_ref")]
        public string BaseRef { get; set; }

        [JsonPropertyName("head_ref")]
        public string HeadRef { get; set; }

        [JsonPropertyName("base_sha")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? BaseSha { get; set; }

        [JsonPropertyName("head_sha")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HeadSha { get; set; }

        [JsonPropertyName("html_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HtmlUrl { get; set; }

        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ApiUrl { get; set; }

        // High‑level summary counts (if provided by GitHub)
        [JsonPropertyName("total_additions")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalAdditions { get; set; }

        [JsonPropertyName("total_removals")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalRemovals { get; set; }

        [JsonPropertyName("total_changes")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? TotalChanges { get; set; }

        [JsonPropertyName("diff")]
        public DependencyDiff Diff { get; set; }
    }

    public sealed class DependencyDiff
    {
        [JsonPropertyName("added")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencyRecord[]? Added { get; set; }

        [JsonPropertyName("removed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencyRecord[]? Removed { get; set; }

        [JsonPropertyName("changed")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencyChange[]? Changed { get; set; }
    }

    // Represents a dependency that is only present in head (added) or only in base (removed)
    public sealed class DependencyRecord
    {
        // Reuse DependabotPackage for package identification (ecosystem + name)
        [JsonPropertyName("package")]
        public DependabotPackage Package { get; set; }

        [JsonPropertyName("manifest_path")]
        public string ManifestPath { get; set; }

        [JsonPropertyName("scope")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scope { get; set; }

        [JsonPropertyName("license")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencyLicense? License { get; set; }

        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; }
    }

    // Represents a dependency whose version changed between base and head
    public sealed class DependencyChange
    {
        [JsonPropertyName("package")]
        public DependabotPackage Package { get; set; }

        [JsonPropertyName("manifest_path")]
        public string ManifestPath { get; set; }

        [JsonPropertyName("scope")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scope { get; set; }

        [JsonPropertyName("license")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencyLicense? License { get; set; }

        [JsonPropertyName("previous_version")]
        public string PreviousVersion { get; set; }

        [JsonPropertyName("new_version")]
        public string NewVersion { get; set; }

        // GitHub may provide a semver diff classification (e.g., "major","minor","patch")
        [JsonPropertyName("change_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChangeType { get; set; }

        [JsonPropertyName("version_comparison")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public VersionComparison? VersionComparison { get; set; }
    }

    public sealed class DependencyLicense
    {
        [JsonPropertyName("spdx_id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SpdxId { get; set; }

        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }
    }

    public sealed class VersionComparison
    {
        [JsonPropertyName("semver")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public SemVerComparison? SemVer { get; set; }
    }

    public sealed class SemVerComparison
    {
        // e.g., "major", "minor", "patch", or null if not semver
        [JsonPropertyName("change_type")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ChangeType { get; set; }
    }

    // Dependency Graph snapshot response (POST /repos/{owner}/{repo}/dependency-graph/snapshots)
    // These models intentionally keep fields optional (nullable) for forward compatibility.
    public sealed class DependencySnapshotResponse
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("version")]
        public int Version { get; set; }

        [JsonPropertyName("created_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? CreatedAt { get; set; }

        [JsonPropertyName("expires_at")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ExpiresAt { get; set; }

        [JsonPropertyName("sha")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Sha { get; set; }

        // Git ref (e.g. refs/heads/main)
        [JsonPropertyName("ref")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Ref { get; set; }

        [JsonPropertyName("job")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencySnapshotJob? Job { get; set; }

        [JsonPropertyName("detector")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencySnapshotDetector? Detector { get; set; }

        // Arbitrary key/value metadata supplied when creating the snapshot
        [JsonPropertyName("metadata")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, string>? Metadata { get; set; }

        // Manifests keyed by manifest path (e.g. "src/Project.csproj", "package.json")
        [JsonPropertyName("manifests")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, DependencySnapshotManifest>? Manifests { get; set; }

        // When the API returns an error object instead of a normal snapshot (defensive)
        [JsonPropertyName("message")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Message { get; set; }
    }

    public sealed class DependencySnapshotJob
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Id { get; set; }

        [JsonPropertyName("correlator")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Correlator { get; set; }

        [JsonPropertyName("html_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? HtmlUrl { get; set; }
    }

    public sealed class DependencySnapshotDetector
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; }

        [JsonPropertyName("url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Url { get; set; }
    }

    public sealed class DependencySnapshotManifest
    {
        [JsonPropertyName("name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Name { get; set; }

        [JsonPropertyName("file")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public DependencySnapshotFile? File { get; set; }

        // Dependencies for this manifest keyed by package alias/name
        [JsonPropertyName("dependencies")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public Dictionary<string, DependencySnapshotDependency>? Dependencies { get; set; }
    }

    public sealed class DependencySnapshotFile
    {
        // Path within the repo
        [JsonPropertyName("source_location")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? SourceLocation { get; set; }
    }

    public sealed class DependencySnapshotDependency
    {
        // Package URL (purl) if supplied (e.g., pkg:npm/left-pad@1.0.0)
        [JsonPropertyName("package_url")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? PackageUrl { get; set; }

        // direct | indirect
        [JsonPropertyName("relationship")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Relationship { get; set; }

        // runtime | development | optional (etc.)
        [JsonPropertyName("scope")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Scope { get; set; }

        // Version string actually resolved
        [JsonPropertyName("version")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Version { get; set; }

        // Original requirements/range (if provided)
        [JsonPropertyName("requirements")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Requirements { get; set; }

        // Downstream dependencies (by name) this dependency brings in
        [JsonPropertyName("dependencies")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string[]? Dependencies { get; set; }
    }
    #endregion
}
