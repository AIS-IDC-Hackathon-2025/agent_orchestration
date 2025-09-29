using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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
}
