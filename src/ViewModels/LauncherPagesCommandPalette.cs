using System;
using System.Collections.Generic;

namespace SourceGit.ViewModels
{
    public class LauncherPagesCommandPalette : ICommandPalette
    {
        public List<LauncherPage> VisiblePages
        {
            get => _visiblePages;
            private set => SetProperty(ref _visiblePages, value);
        }

        public List<RepositoryNode> VisibleRepos
        {
            get => _visibleRepos;
            private set => SetProperty(ref _visibleRepos, value);
        }

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                if (SetProperty(ref _searchFilter, value))
                    UpdateVisible();
            }
        }

        public LauncherPage SelectedPage
        {
            get => _selectedPage;
            set
            {
                if (SetProperty(ref _selectedPage, value) && value != null)
                    SelectedRepo = null;
            }
        }

        public RepositoryNode SelectedRepo
        {
            get => _selectedRepo;
            set
            {
                if (SetProperty(ref _selectedRepo, value) && value != null)
                    SelectedPage = null;
            }
        }

        public LauncherPagesCommandPalette(Launcher launcher)
        {
            _launcher = launcher;
            UpdateVisible();
        }

        public void ClearFilter()
        {
            SearchFilter = string.Empty;
        }

        public void OpenOrSwitchTo()
        {
            _visiblePages.Clear();
            _visibleRepos.Clear();
            Close();

            if (_selectedRepo != null)
                Dashboard.Instance.SelectRepository(_selectedRepo);
            else if (_selectedPage != null)
                _launcher.ActivePage = _selectedPage;
        }

        private void UpdateVisible()
        {
            var repos = new List<RepositoryNode>();
            CollectVisibleRepository(repos, Preferences.Instance.RepositoryNodes);

            var autoSelectRepo = _selectedRepo;
            if (_selectedRepo == null || !repos.Contains(_selectedRepo))
                autoSelectRepo = repos.Count > 0 ? repos[0] : null;

            VisiblePages = [];
            VisibleRepos = repos;
            SelectedPage = null;
            SelectedRepo = autoSelectRepo;
        }

        private void CollectVisibleRepository(List<RepositoryNode> outs, List<RepositoryNode> nodes)
        {
            foreach (var node in nodes)
            {
                if (!node.IsRepository)
                {
                    CollectVisibleRepository(outs, node.SubNodes);
                    continue;
                }

                if (string.IsNullOrEmpty(_searchFilter) ||
                    node.Id.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
                    node.Name.Contains(_searchFilter, StringComparison.OrdinalIgnoreCase))
                    outs.Add(node);
            }
        }

        private Launcher _launcher = null;
        private List<LauncherPage> _visiblePages = [];
        private List<RepositoryNode> _visibleRepos = [];
        private string _searchFilter = string.Empty;
        private LauncherPage _selectedPage = null;
        private RepositoryNode _selectedRepo = null;
    }
}
