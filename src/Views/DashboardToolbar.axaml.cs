using System;
using System.IO;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace SourceGit.Views
{
    public partial class DashboardToolbar : UserControl
    {
        public DashboardToolbar()
        {
            InitializeComponent();
        }

        private async void OnAddRepository(object sender, RoutedEventArgs e)
        {
            var topLevel = TopLevel.GetTopLevel(this);
            if (topLevel == null)
                return;

            var options = new FolderPickerOpenOptions() { AllowMultiple = false };

            try
            {
                var selected = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
                if (selected.Count == 1)
                {
                    var folder = selected[0];
                    var folderPath = folder is { Path: { IsAbsoluteUri: true } path } ? path.LocalPath : folder?.Path.ToString();
                    var repoPath = await ViewModels.Dashboard.Instance.GetRepositoryRootAsync(folderPath);

                    // Fallback: check for .git directly if git command failed
                    if (string.IsNullOrEmpty(repoPath) && !string.IsNullOrEmpty(folderPath))
                    {
                        var gitDir = System.IO.Path.Combine(folderPath, ".git");
                        if (System.IO.Directory.Exists(gitDir) || System.IO.File.Exists(gitDir))
                            repoPath = folderPath;
                    }

                    if (!string.IsNullOrEmpty(repoPath))
                    {
                        await ViewModels.Dashboard.Instance.AddRepositoryAsync(repoPath);
                    }
                    else
                    {
                        App.RaiseException(string.Empty, "Selected folder is not a valid Git repository.");
                    }
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

            var options = new FolderPickerOpenOptions() { AllowMultiple = false };

            try
            {
                var selected = await topLevel.StorageProvider.OpenFolderPickerAsync(options);
                if (selected.Count == 1)
                {
                    var folder = selected[0];
                    var folderPath = folder is { Path: { IsAbsoluteUri: true } path } ? path.LocalPath : folder?.Path.ToString();
                    var found = await ViewModels.Dashboard.Instance.ScanFolderAsync(folderPath);
                    if (found == 0)
                    {
                        App.RaiseException(string.Empty, "No Git repositories found in the selected folder.");
                    }
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
    }
}
