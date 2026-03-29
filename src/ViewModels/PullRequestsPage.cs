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
                        var dc = new DiffContext(_repo.FullPath, new Models.DiffOption(baseRef, headRef, change));
                        dc.IsPullRequestDiff = true;
                        DiffContext = dc;
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

        public async Task<bool> AddInlineCommentAsync(string filePath, int line, string comment)
        {
            if (_client == null || _selectedPR == null || string.IsNullOrWhiteSpace(comment))
                return false;

            return await _client.CreateReviewCommentAsync(
                _selectedPR.Number,
                comment,
                _selectedPR.Head.Sha,
                filePath,
                line);
        }

        public async Task ApproveAsync()
        {
            if (_client == null || _selectedPR == null)
                return;

            var (success, error) = await _client.SubmitReviewAsync(_selectedPR.Number, "APPROVE", string.Empty);
            if (!success)
                App.RaiseException(string.Empty, $"Failed to approve: {error}");
            else
                await LoadPullRequestDetail(_selectedPR);
        }

        public async Task RequestChangesAsync()
        {
            if (_client == null || _selectedPR == null)
                return;

            var (success, error) = await _client.SubmitReviewAsync(_selectedPR.Number, "REQUEST_CHANGES", "Changes requested");
            if (!success)
                App.RaiseException(string.Empty, $"Failed to request changes: {error}");
            else
                await LoadPullRequestDetail(_selectedPR);
        }

        public async Task MergeAsync(string method)
        {
            if (_client == null || _selectedPR == null)
                return;

            var (success, error) = await _client.MergePullRequestAsync(_selectedPR.Number, method);
            if (!success)
                App.RaiseException(string.Empty, $"Failed to merge: {error}");
            else
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

            // Load comments and reviews from GitHub API (in parallel)
            var commentsTask = _client.GetCommentsAsync(pr.Number);
            var reviewsTask = _client.GetReviewsAsync(pr.Number);
            await Task.WhenAll(commentsTask, reviewsTask);
            var comments = commentsTask.Result;
            var reviews = reviewsTask.Result;

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
            _client = Models.GitHubClient.TryCreate(_repo.Remotes);
            if (_client != null)
                OnPropertyChanged(nameof(IsGitHubRepo));
        }

        private readonly Repository _repo;
        private Models.GitHubClient _client;
        private Models.GitHubPullRequest _selectedPR;
        private List<Models.Change> _changes;
        private List<Models.Change> _selectedChanges;
        private DiffContext _diffContext;
        private bool _isLoading;
        private string _filterState = "open";
    }
}
