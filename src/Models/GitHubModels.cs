using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SourceGit.Models
{
    public class GitHubUser
    {
        [JsonPropertyName("login")]
        public string Login { get; set; } = string.Empty;
        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; } = string.Empty;
    }

    public class GitHubLabel
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("color")]
        public string Color { get; set; } = string.Empty;
    }

    public class GitHubPullRequest
    {
        [JsonPropertyName("number")]
        public int Number { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        [JsonPropertyName("draft")]
        public bool IsDraft { get; set; }
        [JsonPropertyName("mergeable")]
        public bool? Mergeable { get; set; }
        [JsonPropertyName("mergeable_state")]
        public string MergeableState { get; set; } = string.Empty;
        [JsonPropertyName("user")]
        public GitHubUser User { get; set; }
        [JsonPropertyName("head")]
        public GitHubBranchRef Head { get; set; }
        [JsonPropertyName("base")]
        public GitHubBranchRef Base { get; set; }
        [JsonPropertyName("labels")]
        public List<GitHubLabel> Labels { get; set; } = [];
        [JsonPropertyName("comments")]
        public int CommentsCount { get; set; }
        [JsonPropertyName("review_comments")]
        public int ReviewCommentsCount { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [JsonPropertyName("merged_at")]
        public DateTime? MergedAt { get; set; }
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
    }

    public class GitHubBranchRef
    {
        [JsonPropertyName("ref")]
        public string Ref { get; set; } = string.Empty;
        [JsonPropertyName("sha")]
        public string Sha { get; set; } = string.Empty;
        [JsonPropertyName("label")]
        public string Label { get; set; } = string.Empty;
    }

    public class GitHubComment
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        [JsonPropertyName("user")]
        public GitHubUser User { get; set; }
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class GitHubReview
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("state")]
        public string State { get; set; } = string.Empty;
        [JsonPropertyName("body")]
        public string Body { get; set; } = string.Empty;
        [JsonPropertyName("user")]
        public GitHubUser User { get; set; }
        [JsonPropertyName("submitted_at")]
        public DateTime? SubmittedAt { get; set; }
    }

    public class GitHubWorkflowRun
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; } = string.Empty;
        [JsonPropertyName("head_branch")]
        public string HeadBranch { get; set; } = string.Empty;
        [JsonPropertyName("head_sha")]
        public string HeadSha { get; set; } = string.Empty;
        [JsonPropertyName("event")]
        public string Event { get; set; } = string.Empty;
        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
        [JsonPropertyName("updated_at")]
        public DateTime? UpdatedAt { get; set; }
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;
        [JsonPropertyName("run_number")]
        public int RunNumber { get; set; }
    }

    public class GitHubWorkflowRunsResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("workflow_runs")]
        public List<GitHubWorkflowRun> WorkflowRuns { get; set; } = [];
    }

    public class GitHubJob
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; } = string.Empty;
        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }
        [JsonPropertyName("steps")]
        public List<GitHubStep> Steps { get; set; } = [];
    }

    public class GitHubJobsResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("jobs")]
        public List<GitHubJob> Jobs { get; set; } = [];
    }

    public class GitHubStep
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; } = string.Empty;
        [JsonPropertyName("number")]
        public int Number { get; set; }
        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }
    }
}
