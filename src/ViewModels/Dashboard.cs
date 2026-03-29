using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Avalonia.Collections;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.ViewModels
{
    public enum DashboardSortMode
    {
        Alphabetical = 0,
        LastModified = 1,
    }

    public class Dashboard : ObservableObject
    {
        public static Dashboard Instance { get; } = new();

        public AvaloniaList<RepositoryNode> Repositories
        {
            get;
            private set;
        } = [];

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                if (SetProperty(ref _searchFilter, value))
                    Refresh();
            }
        }

        public DashboardSortMode SortMode
        {
            get => Preferences.Instance.DashboardSort;
            set
            {
                if (Preferences.Instance.DashboardSort != value)
                {
                    Preferences.Instance.DashboardSort = value;
                    OnPropertyChanged();
                    Refresh();
                }
            }
        }

        public Repository ActiveRepository
        {
            get => _activeRepository;
            private set => SetProperty(ref _activeRepository, value);
        }

        public RepositoryNode ActiveNode
        {
            get => _activeNode;
            private set => SetProperty(ref _activeNode, value);
        }

        public string ParentRepositoryPath
        {
            get => _parentRepositoryPath;
            private set
            {
                if (SetProperty(ref _parentRepositoryPath, value))
                {
                    OnPropertyChanged(nameof(ParentRepositoryName));
                    OnPropertyChanged(nameof(IsSubmodule));
                }
            }
        }

        public string ParentRepositoryName => _parentRepositoryPath != null ? Path.GetFileName(_parentRepositoryPath) : null;

        public bool IsSubmodule => !string.IsNullOrEmpty(_parentRepositoryPath);

        public Dashboard()
        {
            EnsureGitConfigured();
            Refresh();
        }

        public void Refresh()
        {
            var repos = new List<RepositoryNode>();
            CollectRepositories(repos, Preferences.Instance.RepositoryNodes, string.IsNullOrWhiteSpace(_searchFilter) ? null : _searchFilter);

            if (Preferences.Instance.DashboardSort == DashboardSortMode.LastModified)
            {
                var timestamps = new Dictionary<string, DateTime>(repos.Count);
                foreach (var r in repos)
                {
                    var di = new DirectoryInfo(r.Id);
                    timestamps[r.Id] = di.Exists ? di.LastWriteTime : DateTime.MinValue;
                }
                repos.Sort((a, b) => timestamps[b.Id].CompareTo(timestamps[a.Id]));
            }
            else
            {
                repos.Sort((a, b) => string.Compare(a.Name, b.Name, StringComparison.OrdinalIgnoreCase));
            }

            Repositories.Clear();
            Repositories.AddRange(repos);
        }

        public async Task UpdateStatusAsync(bool force, CancellationToken? token)
        {
            EnsureGitConfigured();

            var allRepos = new List<RepositoryNode>();
            CollectRepositories(allRepos, Preferences.Instance.RepositoryNodes, null);

            foreach (var node in allRepos)
            {
                if (token is { IsCancellationRequested: true })
                    break;

                await node.UpdateStatusAsync(force, token);
            }

            Refresh();
        }

        public void ClearSearchFilter()
        {
            SearchFilter = string.Empty;
        }

        public void SelectRepository(RepositoryNode node, string parentRepoPath = null)
        {
            if (node == null || !node.IsRepository)
                return;

            if (_activeNode?.Id == node.Id && _activeRepository != null)
                return;

            // Close previous repo without clearing ActiveNode (avoids selection loop)
            if (_activeRepository != null)
            {
                _activeRepository.Close();
                ActiveRepository = null;
            }

            if (!Path.Exists(node.Id))
            {
                App.RaiseException(node.Id, "Repository does NOT exist any more. Please remove it.");
                return;
            }

            var isBare = new Commands.IsBareRepository(node.Id).GetResult();
            var gitDir = isBare ? node.Id : GetRepositoryGitDir(node.Id);
            if (string.IsNullOrEmpty(gitDir))
            {
                App.RaiseException(node.Id, "Given path is not a valid git repository!");
                return;
            }

            var repo = new Repository(isBare, node.Id, gitDir);
            repo.Open();

            ParentRepositoryPath = parentRepoPath;

            ActiveNode = node;
            ActiveRepository = repo;
        }

        public void GoBackToParent()
        {
            if (string.IsNullOrEmpty(_parentRepositoryPath))
                return;

            var parentNode = Preferences.Instance.FindNode(_parentRepositoryPath) ??
                new RepositoryNode
                {
                    Id = _parentRepositoryPath,
                    Name = Path.GetFileName(_parentRepositoryPath),
                    Bookmark = 0,
                    IsRepository = true,
                };

            SelectRepository(parentNode);
        }

        public void CloseActiveRepository()
        {
            if (_activeRepository != null)
            {
                _activeRepository.Close();
                ActiveRepository = null;
            }
            ActiveNode = null;
        }

        public void RemoveRepository(RepositoryNode node)
        {
            Preferences.Instance.RemoveNode(node, true);
            Refresh();
        }

        public async Task<string> GetRepositoryRootAsync(string path)
        {
            EnsureGitConfigured();

            if (!Preferences.Instance.IsGitConfigured())
                return null;

            var root = path;
            if (!Directory.Exists(root))
            {
                if (File.Exists(root))
                    root = Path.GetDirectoryName(root);
                else
                    return null;
            }

            var isBare = await new Commands.IsBareRepository(root).GetResultAsync();
            if (isBare)
                return root;

            var rs = await new Commands.QueryRepositoryRootPath(root).GetResultAsync();
            if (!rs.IsSuccess || string.IsNullOrWhiteSpace(rs.StdOut))
                return null;

            return rs.StdOut.Trim();
        }

        public async Task AddRepositoryAsync(string path)
        {
            EnsureGitConfigured();
            var node = Preferences.Instance.FindOrAddNodeByRepositoryPath(path, null, false);
            await node.UpdateStatusAsync(true, null);
            Preferences.Instance.Save();
            Refresh();
        }

        public async Task<int> ScanFolderAsync(string folderPath)
        {
            if (!Directory.Exists(folderPath))
                return 0;

            EnsureGitConfigured();

            var dir = new DirectoryInfo(folderPath);
            var addedNodes = new List<RepositoryNode>();

            foreach (var subdir in dir.GetDirectories())
            {
                if (subdir.Name.StartsWith(".", StringComparison.Ordinal))
                    continue;

                var gitDir = Path.Combine(subdir.FullName, ".git");
                if (Directory.Exists(gitDir) || File.Exists(gitDir))
                {
                    var repoPath = subdir.FullName;
                    var node = Preferences.Instance.FindOrAddNodeByRepositoryPath(repoPath, null, false);
                    addedNodes.Add(node);
                }
            }

            if (addedNodes.Count > 0)
            {
                Refresh();

                foreach (var node in addedNodes)
                    await node.UpdateStatusAsync(true, null);

                Preferences.Instance.Save();
                Refresh();
            }

            return addedNodes.Count;
        }

        public void SortByName()
        {
            SortMode = DashboardSortMode.Alphabetical;
        }

        public void SortByLastModified()
        {
            SortMode = DashboardSortMode.LastModified;
        }

        private void CollectRepositories(List<RepositoryNode> result, List<RepositoryNode> nodes, string filter = null)
        {
            foreach (var node in nodes)
            {
                if (node.IsRepository)
                {
                    if (filter == null ||
                        node.Name.Contains(filter, StringComparison.OrdinalIgnoreCase) ||
                        node.Id.Contains(filter, StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(node);
                    }
                }
                else
                {
                    CollectRepositories(result, node.SubNodes, filter);
                }
            }
        }

        private void EnsureGitConfigured()
        {
            if (_gitConfigured)
                return;

            if (Preferences.Instance.IsGitConfigured())
            {
                _gitConfigured = true;
                return;
            }

            var found = Native.OS.FindGitExecutable();
            if (!string.IsNullOrEmpty(found) && File.Exists(found))
            {
                Preferences.Instance.GitInstallPath = found;
                Preferences.Instance.Save();
                _gitConfigured = true;
                return;
            }

            // Fallback: try common Windows paths
            var candidates = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Git", "bin", "git.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Git", "cmd", "git.exe"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Git", "bin", "git.exe"),
            };

            foreach (var candidate in candidates)
            {
                if (File.Exists(candidate))
                {
                    Preferences.Instance.GitInstallPath = candidate;
                    Preferences.Instance.Save();
                    break;
                }
            }

            if (Preferences.Instance.IsGitConfigured())
            {
                _gitConfigured = true;

                // Mark all directories as safe to avoid "dubious ownership" errors
                // (common when repos are cloned from WSL or run as different user)
                try
                {
                    var psi = new System.Diagnostics.ProcessStartInfo
                    {
                        FileName = Native.OS.GitExecutable,
                        Arguments = "config --global --get-all safe.directory",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                    };
                    using var proc = System.Diagnostics.Process.Start(psi);
                    var output = proc.StandardOutput.ReadToEnd();
                    proc.WaitForExit();

                    if (!output.Split('\n').Any(line => line.Trim() == "*"))
                    {
                        var psi2 = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = Native.OS.GitExecutable,
                            Arguments = "config --global --add safe.directory *",
                            UseShellExecute = false,
                            CreateNoWindow = true,
                        };
                        using var proc2 = System.Diagnostics.Process.Start(psi2);
                        proc2.WaitForExit();
                    }
                }
                catch
                {
                    // ignore
                }
            }
        }

        private string GetRepositoryGitDir(string repo)
        {
            var fullpath = Path.Combine(repo, ".git");
            if (Directory.Exists(fullpath))
            {
                if (Directory.Exists(Path.Combine(fullpath, "refs")) &&
                    Directory.Exists(Path.Combine(fullpath, "objects")) &&
                    File.Exists(Path.Combine(fullpath, "HEAD")))
                    return fullpath;
                return null;
            }

            if (File.Exists(fullpath))
            {
                var redirect = File.ReadAllText(fullpath).Trim();
                if (redirect.StartsWith("gitdir: ", StringComparison.Ordinal))
                    redirect = redirect.Substring(8);
                if (!Path.IsPathRooted(redirect))
                    redirect = Path.GetFullPath(Path.Combine(repo, redirect));
                if (Directory.Exists(redirect))
                    return redirect;
                return null;
            }

            return new Commands.QueryGitDir(repo).GetResult();
        }

        private bool _gitConfigured;
        private string _searchFilter = string.Empty;
        private Repository _activeRepository = null;
        private RepositoryNode _activeNode = null;
        private string _parentRepositoryPath = null;
    }
}
