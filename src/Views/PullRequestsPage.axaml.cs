using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SourceGit.Views
{
    public partial class PullRequestsPage : UserControl
    {
        public PullRequestsPage()
        {
            InitializeComponent();
        }

        protected override async void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.RefreshAsync();
        }

        private void OnFilterOpen(object s, RoutedEventArgs e) => SetFilter("open");
        private void OnFilterClosed(object s, RoutedEventArgs e) => SetFilter("closed");
        private void OnFilterAll(object s, RoutedEventArgs e) => SetFilter("all");

        private void SetFilter(string state)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                vm.FilterState = state;
        }

        private async void OnRefresh(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.RefreshAsync();
        }

        private async void OnAddComment(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.AddCommentAsync();
        }

        private async void OnApprove(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.ApproveAsync();
        }

        private async void OnRequestChanges(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.RequestChangesAsync();
        }

        private async void OnCommentReview(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.CommentReviewAsync();
        }

        private async void OnMerge(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.MergeAsync("merge");
        }

        private async void OnSquash(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.MergeAsync("squash");
        }

        private async void OnRebase(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PullRequestsPage vm)
                await vm.MergeAsync("rebase");
        }
    }
}
