using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Avalonia.Collections;

namespace SourceGit.ViewModels
{
    public class DashboardCheckout : Popup
    {
        public Repository Repo { get; }

        public AvaloniaList<Models.Branch> FilteredBranches { get; } = [];

        public string SearchFilter
        {
            get => _searchFilter;
            set
            {
                if (SetProperty(ref _searchFilter, value))
                    RefreshFilter();
            }
        }

        public Models.Branch SelectedBranch
        {
            get => _selectedBranch;
            set => SetProperty(ref _selectedBranch, value);
        }

        public bool IsNewBranch
        {
            get => _isNewBranch;
            private set => SetProperty(ref _isNewBranch, value);
        }

        public DashboardCheckout(Repository repo)
        {
            Repo = repo;
            _repo = repo;
            _allBranches = repo.Branches ?? [];
            RefreshFilter();
        }

        public override async Task<bool> Sure()
        {
            using var lockWatcher = _repo.LockWatcher();

            // If a branch is selected from the list, use it
            var target = _selectedBranch;

            if (target == null && !string.IsNullOrWhiteSpace(_searchFilter))
            {
                // Try to find exact match by name
                target = _allBranches.FirstOrDefault(b =>
                    b.FriendlyName.Equals(_searchFilter, StringComparison.OrdinalIgnoreCase) ||
                    b.Name.Equals(_searchFilter, StringComparison.OrdinalIgnoreCase));
            }

            if (target != null)
            {
                if (target.IsLocal)
                    return await CheckoutLocalAsync(target.Name);
                else
                    return await CheckoutRemoteAsync(target);
            }

            // No match — create new branch from HEAD
            if (!string.IsNullOrWhiteSpace(_searchFilter))
                return await CreateAndCheckoutAsync(_searchFilter.Trim());

            return false;
        }

        private async Task<bool> CheckoutLocalAsync(string branchName)
        {
            ProgressDescription = $"Checkout '{branchName}' ...";

            var log = _repo.CreateLog($"Checkout '{branchName}'");
            Use(log);

            var succ = await new Commands.Checkout(_repo.FullPath)
                .Use(log)
                .BranchAsync(branchName, false);

            log.Complete();

            if (succ)
                _repo.MarkBranchesDirtyManually();

            ProgressDescription = "Waiting for branch updated...";
            await Task.Delay(400);
            return succ;
        }

        private async Task<bool> CheckoutRemoteAsync(Models.Branch remoteBranch)
        {
            var localName = remoteBranch.Name;
            ProgressDescription = $"Checkout '{localName}' from '{remoteBranch.FriendlyName}' ...";

            var log = _repo.CreateLog($"Checkout '{localName}' from '{remoteBranch.FriendlyName}'");
            Use(log);

            var succ = await new Commands.Checkout(_repo.FullPath)
                .Use(log)
                .BranchAsync(localName, remoteBranch.FriendlyName, false, false);

            log.Complete();

            if (succ)
                _repo.MarkBranchesDirtyManually();

            ProgressDescription = "Waiting for branch updated...";
            await Task.Delay(400);
            return succ;
        }

        private async Task<bool> CreateAndCheckoutAsync(string branchName)
        {
            ProgressDescription = $"Create and checkout '{branchName}' ...";

            var log = _repo.CreateLog($"Create and checkout '{branchName}'");
            Use(log);

            var succ = await new Commands.Checkout(_repo.FullPath)
                .Use(log)
                .BranchAsync(branchName, "HEAD", false, false);

            log.Complete();

            if (succ)
                _repo.MarkBranchesDirtyManually();

            ProgressDescription = "Waiting for branch updated...";
            await Task.Delay(400);
            return succ;
        }

        private void RefreshFilter()
        {
            FilteredBranches.Clear();

            var filter = _searchFilter ?? string.Empty;
            foreach (var b in _allBranches)
            {
                if (b.IsCurrent)
                    continue;

                if (string.IsNullOrEmpty(filter) ||
                    b.FriendlyName.Contains(filter, StringComparison.OrdinalIgnoreCase))
                {
                    FilteredBranches.Add(b);
                }
            }

            // Check if the filter text is a new branch name (no existing branch matches exactly)
            IsNewBranch = !string.IsNullOrWhiteSpace(filter) &&
                !_allBranches.Any(b =>
                    b.FriendlyName.Equals(filter, StringComparison.OrdinalIgnoreCase) ||
                    b.Name.Equals(filter, StringComparison.OrdinalIgnoreCase));
        }

        private readonly Repository _repo;
        private readonly List<Models.Branch> _allBranches;
        private string _searchFilter = string.Empty;
        private Models.Branch _selectedBranch;
        private bool _isNewBranch;
    }
}
