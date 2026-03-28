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

        public Dashboard()
        {
            EnsureGitConfigured();
            Refresh();
        }

        public void Refresh()
        {
            var repos = new List<RepositoryNode>();
            CollectRepositories(repos, Preferences.Instance.RepositoryNodes);

            Repositories.Clear();
            Repositories.AddRange(repos);
        }

        public async Task UpdateStatusAsync(bool force, CancellationToken? token)
        {
            EnsureGitConfigured();

            // Collect ALL repository nodes (not just filtered ones)
            var allRepos = new List<RepositoryNode>();
            CollectAllRepositories(allRepos, Preferences.Instance.RepositoryNodes);

            foreach (var node in allRepos)
            {
                if (token is { IsCancellationRequested: true })
                    break;

                await node.UpdateStatusAsync(force, token);
            }

            // Refresh the visible list to pick up status changes
            Refresh();
        }

        public void ClearSearchFilter()
        {
            SearchFilter = string.Empty;
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

        public void OpenRepository(RepositoryNode node)
        {
            SelectRepository(node);
        }

        public void SelectRepository(RepositoryNode node)
        {
            if (node == null || !node.IsRepository)
                return;

            // Already selected
            if (_activeNode?.Id == node.Id && _activeRepository != null)
                return;

            // Close previous
            CloseActiveRepository();

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

            ActiveNode = node;
            ActiveRepository = repo;
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

            // Phase 1: discover and add repos (no git status yet)
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
                Preferences.Instance.Save();
                Refresh();

                // Phase 2: query status for all found repos
                foreach (var node in addedNodes)
                    await node.UpdateStatusAsync(true, null);

                Preferences.Instance.Save();
                Refresh();
            }

            return addedNodes.Count;
        }

        private void CollectAllRepositories(List<RepositoryNode> result, List<RepositoryNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsRepository)
                    result.Add(node);
                else
                    CollectAllRepositories(result, node.SubNodes);
            }
        }

        private void CollectRepositories(List<RepositoryNode> result, List<RepositoryNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (node.IsRepository)
                {
                    if (string.IsNullOrWhiteSpace(_searchFilter) ||
                        node.Name.Contains(_searchFilter, System.StringComparison.OrdinalIgnoreCase) ||
                        node.Id.Contains(_searchFilter, System.StringComparison.OrdinalIgnoreCase))
                    {
                        result.Add(node);
                    }
                }
                else
                {
                    CollectRepositories(result, node.SubNodes);
                }
            }
        }

        private void EnsureGitConfigured()
        {
            if (Preferences.Instance.IsGitConfigured())
                return;

            var found = Native.OS.FindGitExecutable();
            if (!string.IsNullOrEmpty(found) && File.Exists(found))
            {
                Preferences.Instance.GitInstallPath = found;
                Preferences.Instance.Save();
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

            // Mark all directories as safe to avoid "dubious ownership" errors
            // (common when repos are cloned from WSL or run as different user)
            if (Preferences.Instance.IsGitConfigured())
            {
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

        private string _searchFilter = string.Empty;
        private Repository _activeRepository = null;
        private RepositoryNode _activeNode = null;
    }
}
