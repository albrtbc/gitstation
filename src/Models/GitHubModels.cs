using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

using CommunityToolkit.Mvvm.ComponentModel;

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

    public class GitHubPullRequest : ObservableObject
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

        [JsonIgnore]
        public string CIConclusion
        {
            get => _ciConclusion;
            set
            {
                SetProperty(ref _ciConclusion, value);
                if (!_hasCI)
                {
                    _hasCI = true;
                    OnPropertyChanged(nameof(HasCI));
                }
            }
        }

        [JsonIgnore]
        public bool HasCI => _hasCI;

        private string _ciConclusion;
        private bool _hasCI;
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

    public class GitHubStepLog : ObservableObject
    {
        public GitHubStep Step { get; set; }
        public string Log { get; set; } = string.Empty;

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isExpanded;
    }

    public class GitHubJobDetail : ObservableObject
    {
        public GitHubJob Job { get; set; }
        public List<GitHubStepLog> StepLogs { get; set; } = [];

        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        private bool _isExpanded;
    }

    public class GitHubCheckRun
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;
        [JsonPropertyName("conclusion")]
        public string Conclusion { get; set; }
        [JsonPropertyName("started_at")]
        public DateTime? StartedAt { get; set; }
        [JsonPropertyName("completed_at")]
        public DateTime? CompletedAt { get; set; }
        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; } = string.Empty;

        [JsonIgnore]
        public string EffectiveConclusion => Status == "completed" ? Conclusion : null;
    }

    public class GitHubCheckRunsResponse
    {
        [JsonPropertyName("total_count")]
        public int TotalCount { get; set; }
        [JsonPropertyName("check_runs")]
        public List<GitHubCheckRun> CheckRuns { get; set; } = [];
    }

    // Request payloads for GitHub API
    public class CreatePullRequestPayload
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("head")]
        public string Head { get; set; }
        [JsonPropertyName("base")]
        public string Base { get; set; }
    }

    public class UpdatePullRequestPayload
    {
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public class MergePullRequestPayload
    {
        [JsonPropertyName("merge_method")]
        public string MergeMethod { get; set; }
    }

    public class CommentPayload
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public class SubmitReviewPayload
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        [JsonPropertyName("body")]
        public string Body { get; set; }
    }

    public class ReviewCommentPayload
    {
        [JsonPropertyName("body")]
        public string Body { get; set; }
        [JsonPropertyName("commit_id")]
        public string CommitId { get; set; }
        [JsonPropertyName("path")]
        public string Path { get; set; }
        [JsonPropertyName("line")]
        public int Line { get; set; }
        [JsonPropertyName("side")]
        public string Side { get; set; }
    }

    [JsonSerializable(typeof(List<GitHubPullRequest>))]
    [JsonSerializable(typeof(GitHubPullRequest))]
    [JsonSerializable(typeof(List<GitHubComment>))]
    [JsonSerializable(typeof(GitHubComment))]
    [JsonSerializable(typeof(List<GitHubReview>))]
    [JsonSerializable(typeof(GitHubWorkflowRunsResponse))]
    [JsonSerializable(typeof(GitHubJobsResponse))]
    [JsonSerializable(typeof(GitHubCheckRunsResponse))]
    [JsonSerializable(typeof(CreatePullRequestPayload))]
    [JsonSerializable(typeof(UpdatePullRequestPayload))]
    [JsonSerializable(typeof(MergePullRequestPayload))]
    [JsonSerializable(typeof(CommentPayload))]
    [JsonSerializable(typeof(SubmitReviewPayload))]
    [JsonSerializable(typeof(ReviewCommentPayload))]
    internal partial class GitHubJsonContext : JsonSerializerContext
    {
    }
}
