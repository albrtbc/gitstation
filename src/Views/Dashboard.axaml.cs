using System;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace SourceGit.Views
{
    public partial class Dashboard : UserControl
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        protected override async void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            await ViewModels.Dashboard.Instance.UpdateStatusAsync(true, _cancellation.Token);
        }

        protected override void OnUnloaded(RoutedEventArgs e)
        {
            _cancellation.Cancel();
            ViewModels.Dashboard.Instance.CloseActiveRepository();
            base.OnUnloaded(e);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!e.Handled && e.Key is Key.Delete or Key.Back)
            {
                if (RepoList.SelectedItem is ViewModels.RepositoryNode node)
                {
                    ViewModels.Dashboard.Instance.RemoveRepository(node);
                    e.Handled = true;
                }
            }
        }

        private void OnRepoSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_isSelecting)
                return;

            _isSelecting = true;
            try
            {
                if (sender is ListBox lb && lb.SelectedItem is ViewModels.RepositoryNode node && node.IsRepository)
                {
                    ViewModels.Dashboard.Instance.SelectRepository(node);
                }
            }
            finally
            {
                _isSelecting = false;
            }
        }

        // Prevents SelectRepository → ActiveNode change → SelectionChanged → SelectRepository re-entry loop
        private bool _isSelecting;

        private void OnRowContextRequested(object sender, ContextRequestedEventArgs e)
        {
            if (sender is Grid { DataContext: ViewModels.RepositoryNode node })
            {
                var menu = new ContextMenu();

                var open = new MenuItem();
                open.Header = "Open Repository";
                open.Icon = App.CreateMenuIcon("Icons.Folder.Open");
                open.Click += (_, ev) =>
                {
                    ViewModels.Dashboard.Instance.SelectRepository(node);
                    ev.Handled = true;
                };
                menu.Items.Add(open);

                menu.Items.Add(new MenuItem() { Header = "-" });

                var checkout = new MenuItem();
                checkout.Header = "Checkout";
                checkout.Icon = App.CreateMenuIcon("Icons.Branch");
                checkout.Click += (_, ev) =>
                {
                    ViewModels.Dashboard.Instance.SelectRepository(node);
                    var page = App.GetLauncher().ActivePage;
                    var repo = ViewModels.Dashboard.Instance.ActiveRepository;
                    if (page != null && page.CanCreatePopup() && repo != null)
                        page.Popup = new ViewModels.DashboardCheckout(repo);
                    ev.Handled = true;
                };
                menu.Items.Add(checkout);

                var pull = new MenuItem();
                pull.Header = "Pull";
                pull.Icon = App.CreateMenuIcon("Icons.Pull");
                pull.Click += (_, ev) =>
                {
                    ViewModels.Dashboard.Instance.SelectRepository(node);
                    var page = App.GetLauncher().ActivePage;
                    var repo = ViewModels.Dashboard.Instance.ActiveRepository;
                    if (page != null && page.CanCreatePopup() && repo != null)
                        page.Popup = new ViewModels.Pull(repo, null);
                    ev.Handled = true;
                };
                menu.Items.Add(pull);

                var push = new MenuItem();
                push.Header = "Push";
                push.Icon = App.CreateMenuIcon("Icons.Push");
                push.Click += (_, ev) =>
                {
                    ViewModels.Dashboard.Instance.SelectRepository(node);
                    var page = App.GetLauncher().ActivePage;
                    var repo = ViewModels.Dashboard.Instance.ActiveRepository;
                    if (page != null && page.CanCreatePopup() && repo != null)
                        page.Popup = new ViewModels.Push(repo, null);
                    ev.Handled = true;
                };
                menu.Items.Add(push);

                menu.Items.Add(new MenuItem() { Header = "-" });

                var explore = new MenuItem();
                explore.Header = App.Text("Repository.Explore");
                explore.Icon = App.CreateMenuIcon("Icons.Explore");
                explore.Click += (_, ev) =>
                {
                    node.OpenInFileManager();
                    ev.Handled = true;
                };
                menu.Items.Add(explore);

                var terminal = new MenuItem();
                terminal.Header = App.Text("Repository.Terminal");
                terminal.Icon = App.CreateMenuIcon("Icons.Terminal");
                terminal.Click += (_, ev) =>
                {
                    node.OpenTerminal();
                    ev.Handled = true;
                };
                menu.Items.Add(terminal);

                var remoteUrl = GetRemoteUrl(node.Id);
                if (remoteUrl != null)
                {
                    var browser = new MenuItem();
                    browser.Header = "Open in Browser";
                    browser.Icon = App.CreateMenuIcon("Icons.Link");
                    browser.Click += (_, ev) =>
                    {
                        Native.OS.OpenBrowser(remoteUrl);
                        ev.Handled = true;
                    };
                    menu.Items.Add(browser);
                }

                menu.Items.Add(new MenuItem() { Header = "-" });

                var remove = new MenuItem();
                remove.Header = "Remove";
                remove.Icon = App.CreateMenuIcon("Icons.Clear");
                remove.Click += (_, ev) =>
                {
                    ViewModels.Dashboard.Instance.RemoveRepository(node);
                    ev.Handled = true;
                };
                menu.Items.Add(remove);

                menu.Open(sender as Grid);
            }

            e.Handled = true;
        }

        private async void OnAddRepository(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var options = new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = false };

            try
            {
                var selected = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
                if (selected.Count == 1)
                {
                    var folder = selected[0];
                    var folderPath = folder is { Path: { IsAbsoluteUri: true } path } ? path.LocalPath : folder?.Path.ToString();
                    var repoPath = await ViewModels.Dashboard.Instance.GetRepositoryRootAsync(folderPath);

                    if (string.IsNullOrEmpty(repoPath) && !string.IsNullOrEmpty(folderPath))
                    {
                        var gitDir = System.IO.Path.Combine(folderPath, ".git");
                        if (System.IO.Directory.Exists(gitDir) || System.IO.File.Exists(gitDir))
                            repoPath = folderPath;
                    }

                    if (!string.IsNullOrEmpty(repoPath))
                        await ViewModels.Dashboard.Instance.AddRepositoryAsync(repoPath);
                    else
                        App.RaiseException(string.Empty, "Selected folder is not a valid Git repository.");
                }
            }
            catch (Exception exception)
            {
                App.RaiseException(string.Empty, $"Failed to open repository: {exception.Message}");
            }

            e.Handled = true;
        }

        private async void OnScanFolder(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var options = new Avalonia.Platform.Storage.FolderPickerOpenOptions() { AllowMultiple = false };

            try
            {
                var selected = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
                if (selected.Count == 1)
                {
                    var folder = selected[0];
                    var folderPath = folder is { Path: { IsAbsoluteUri: true } path } ? path.LocalPath : folder?.Path.ToString();
                    var found = await ViewModels.Dashboard.Instance.ScanFolderAsync(folderPath);
                    if (found == 0)
                        App.RaiseException(string.Empty, "No Git repositories found in the selected folder.");
                }
            }
            catch (Exception exception)
            {
                App.RaiseException(string.Empty, $"Failed to scan folder: {exception.Message}");
            }

            e.Handled = true;
        }

        private async void OnRefreshClicked(object sender, RoutedEventArgs e)
        {
            ViewModels.Dashboard.Instance.Refresh();
            await ViewModels.Dashboard.Instance.UpdateStatusAsync(true, null);
        }

        private void OnSortAlphabetical(object sender, RoutedEventArgs e)
        {
            ViewModels.Dashboard.Instance.SortByName();
        }

        private void OnSortLastModified(object sender, RoutedEventArgs e)
        {
            ViewModels.Dashboard.Instance.SortByLastModified();
        }

        private static string GetRemoteUrl(string repoPath)
        {
            try
            {
                var config = new Commands.Config(repoPath).Get("remote.origin.url");
                if (string.IsNullOrEmpty(config))
                    return null;

                var url = config.Trim();

                // SSH format: git@github.com:owner/repo.git
                if (url.StartsWith("git@", StringComparison.Ordinal))
                {
                    url = url.Replace(':', '/').Replace("git@", "https://");
                }

                if (url.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                    url = url[..^4];

                return url.StartsWith("http", StringComparison.OrdinalIgnoreCase) ? url : null;
            }
            catch
            {
                return null;
            }
        }

        private CancellationTokenSource _cancellation = new CancellationTokenSource();
    }
}
