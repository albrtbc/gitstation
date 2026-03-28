using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Avalonia.Collections;
using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.ViewModels
{
    public class PipelinesPage : ObservableObject
    {
        public AvaloniaList<Models.GitHubWorkflowRun> Runs { get; } = [];

        public Models.GitHubWorkflowRun SelectedRun
        {
            get => _selectedRun;
            set
            {
                if (SetProperty(ref _selectedRun, value) && value != null)
                    LoadRunDetail(value.Id);
            }
        }

        public AvaloniaList<Models.GitHubJob> Jobs { get; } = [];

        public Models.GitHubJob SelectedJob
        {
            get => _selectedJob;
            set
            {
                if (SetProperty(ref _selectedJob, value) && value != null)
                    LoadJobLogs(value.Id);
            }
        }

        public string JobLogs
        {
            get => _jobLogs;
            private set => SetProperty(ref _jobLogs, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            private set => SetProperty(ref _isLoading, value);
        }

        public bool IsGitHubRepo => _client != null;

        public PipelinesPage(Repository repo)
        {
            _repo = repo;
            InitializeClient();
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
                var runs = await _client.GetWorkflowRunsAsync(50);
                Dispatcher.UIThread.Invoke(() =>
                {
                    Runs.Clear();
                    Runs.AddRange(runs);
                });
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async void LoadRunDetail(long runId)
        {
            if (_client == null)
                return;

            var jobs = await _client.GetJobsAsync(runId);
            Dispatcher.UIThread.Invoke(() =>
            {
                Jobs.Clear();
                Jobs.AddRange(jobs);
                JobLogs = string.Empty;
                SelectedJob = null;
            });
        }

        private async void LoadJobLogs(long jobId)
        {
            if (_client == null)
                return;

            var logs = await _client.GetJobLogsAsync(jobId);
            Dispatcher.UIThread.Invoke(() => JobLogs = logs);
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
        private Models.GitHubWorkflowRun _selectedRun;
        private Models.GitHubJob _selectedJob;
        private string _jobLogs = string.Empty;
        private bool _isLoading;
    }
}
