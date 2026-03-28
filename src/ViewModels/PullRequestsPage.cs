using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.ViewModels
{
    public class PullRequestsPage : ObservableObject
    {
        public AvaloniaList<Models.GitHubPullRequest> PullRequests { get; } = [];

        public Models.GitHubPullRequest SelectedPullRequest
        {
            get => _selectedPR;
            set
            {
                if (SetProperty(ref _selectedPR, value))
                {
                    if (value != null)
                        _ = LoadPullRequestDetail(value);
                    else
                        ClearDetail();
                }
            }
        }

        public List<Models.Change> Changes
        {
            get => _changes;
            private set => SetProperty(ref _changes, value);
        }

        public List<Models.Change> SelectedChanges
        {
            get => _selectedChanges;
            set
            {
                if (SetProperty(ref _selectedChanges, value))
                {
                    if (value != null && value.Count == 1 && _selectedPR != null)
                    {
                        var change = value[0];
                        var baseRef = $"origin/{_selectedPR.Base.Ref}";
                        var headRef = $"origin/{_selectedPR.Head.Ref}";
                        DiffContext = new DiffContext(_repo.FullPath, new Models.DiffOption(baseRef, headRef, change));
                    }
                    else
                    {
                        DiffContext = null;
                    }
                }
            }
        }

        public DiffContext DiffContext
        {
            get => _diffContext;
            private set => SetProperty(ref _diffContext, value);
        }

        public AvaloniaList<Models.GitHubComment> Comments { get; } = [];
        public AvaloniaList<Models.GitHubReview> Reviews { get; } = [];

        public string NewComment
        {
            get => _newComment;
            set => SetProperty(ref _newComment, value);
        }

        public string ReviewBody
        {
            get => _reviewBody;
            set => SetProperty(ref _reviewBody, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public string FilterState
        {
            get => _filterState;
            set
            {
                if (SetProperty(ref _filterState, value))
                    _ = RefreshAsync();
            }
        }

        public bool IsGitHubRepo => _client != null;

        public PullRequestsPage(Repository repo)
        {
            _repo = repo;
        }

        public async Task RefreshAsync()
        {
            if (_client == null)
                InitializeClient();

            if (_client == null)
                return;

            IsLoading = true;
            try
            {
                var prs = await _client.GetPullRequestsAsync(_filterState);
                Dispatcher.UIThread.Invoke(() =>
                {
                    PullRequests.Clear();
                    PullRequests.AddRange(prs);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task AddCommentAsync()
        {
            if (_client == null || _selectedPR == null || string.IsNullOrWhiteSpace(_newComment))
                return;

            var comment = await _client.AddCommentAsync(_selectedPR.Number, _newComment);
            if (comment != null)
            {
                Dispatcher.UIThread.Invoke(() => Comments.Add(comment));
                NewComment = string.Empty;
            }
        }

        public async Task ApproveAsync()
        {
            if (_client == null || _selectedPR == null)
                return;

            await _client.SubmitReviewAsync(_selectedPR.Number, "APPROVE", _reviewBody ?? string.Empty);
            ReviewBody = string.Empty;
            await LoadPullRequestDetail(_selectedPR);
        }

        public async Task RequestChangesAsync()
        {
            if (_client == null || _selectedPR == null || string.IsNullOrWhiteSpace(_reviewBody))
                return;

            await _client.SubmitReviewAsync(_selectedPR.Number, "REQUEST_CHANGES", _reviewBody);
            ReviewBody = string.Empty;
            await LoadPullRequestDetail(_selectedPR);
        }

        public async Task CommentReviewAsync()
        {
            if (_client == null || _selectedPR == null || string.IsNullOrWhiteSpace(_reviewBody))
                return;

            await _client.SubmitReviewAsync(_selectedPR.Number, "COMMENT", _reviewBody);
            ReviewBody = string.Empty;
            await LoadPullRequestDetail(_selectedPR);
        }

        public async Task MergeAsync(string method)
        {
            if (_client == null || _selectedPR == null)
                return;

            await _client.MergePullRequestAsync(_selectedPR.Number, method);
            await RefreshAsync();
        }

        private async Task LoadPullRequestDetail(Models.GitHubPullRequest pr)
        {
            if (_client == null)
                return;

            // Load file changes using local git (fast)
            try
            {
                var baseRef = $"origin/{pr.Base.Ref}";
                var headRef = $"origin/{pr.Head.Ref}";
                var changes = await new Commands.CompareRevisions(_repo.FullPath, baseRef, headRef).ReadAsync();
                Dispatcher.UIThread.Invoke(() => Changes = changes);
            }
            catch
            {
                Dispatcher.UIThread.Invoke(() => Changes = []);
            }

            // Load comments and reviews from GitHub API
            var comments = await _client.GetCommentsAsync(pr.Number);
            var reviews = await _client.GetReviewsAsync(pr.Number);

            Dispatcher.UIThread.Invoke(() =>
            {
                Comments.Clear();
                Comments.AddRange(comments);
                Reviews.Clear();
                Reviews.AddRange(reviews);
            });
        }

        private void ClearDetail()
        {
            Changes = null;
            SelectedChanges = null;
            DiffContext = null;
            Comments.Clear();
            Reviews.Clear();
        }

        private void InitializeClient()
        {
            var token = Models.GitHubClient.ResolveToken();
            if (string.IsNullOrEmpty(token) || _repo.Remotes == null)
                return;

            foreach (var remote in _repo.Remotes)
            {
                if (remote.URL.Contains("github.com", StringComparison.OrdinalIgnoreCase))
                {
                    var (owner, repo) = Models.GitHubClient.ParseRemoteUrl(remote.URL);
                    if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo))
                    {
                        _client = new Models.GitHubClient(token, owner, repo);
                        OnPropertyChanged(nameof(IsGitHubRepo));
                        return;
                    }
                }
            }
        }

        private readonly Repository _repo;
        private Models.GitHubClient _client;
        private Models.GitHubPullRequest _selectedPR;
        private List<Models.Change> _changes;
        private List<Models.Change> _selectedChanges;
        private DiffContext _diffContext;
        private string _newComment = string.Empty;
        private string _reviewBody = string.Empty;
        private bool _isLoading;
        private string _filterState = "open";
    }
}
