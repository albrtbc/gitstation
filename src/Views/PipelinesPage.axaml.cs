using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SourceGit.Views
{
    public partial class PipelinesPage : UserControl
    {
        public PipelinesPage()
        {
            InitializeComponent();
        }

        protected override async void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (DataContext is ViewModels.PipelinesPage vm)
                await vm.RefreshAsync();
        }

        private async void OnRefresh(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PipelinesPage vm)
                await vm.RefreshAsync();
        }

        private void OnToggleJob(object sender, PointerPressedEventArgs e)
        {
            if (sender is Border { DataContext: Models.GitHubJobDetail jobDetail })
            {
                jobDetail.IsExpanded = !jobDetail.IsExpanded;
                e.Handled = true;
            }
        }

        private void OnToggleStepLog(object sender, PointerPressedEventArgs e)
        {
            if (sender is Border { DataContext: Models.GitHubStepLog stepLog })
            {
                stepLog.IsExpanded = !stepLog.IsExpanded;
                e.Handled = true;
            }
        }

        private void OnRunContextRequested(object sender, ContextRequestedEventArgs e)
        {
            if (sender is Grid { DataContext: Models.GitHubWorkflowRun run } grid &&
                DataContext is ViewModels.PipelinesPage vm)
            {
                var menu = new ContextMenu();

                var rerun = new MenuItem();
                rerun.Header = "Re-run Workflow";
                rerun.Icon = App.CreateMenuIcon("Icons.Loading");
                rerun.Click += async (_, ev) =>
                {
                    await vm.RerunWorkflowAsync(run);
                    ev.Handled = true;
                };
                menu.Items.Add(rerun);

                var openBrowser = new MenuItem();
                openBrowser.Header = "Open in Browser";
                openBrowser.Icon = App.CreateMenuIcon("Icons.Link");
                openBrowser.Click += (_, ev) =>
                {
                    vm.OpenInBrowser(run);
                    ev.Handled = true;
                };
                menu.Items.Add(openBrowser);

                menu.Open(grid);
            }

            e.Handled = true;
        }
    }
}
