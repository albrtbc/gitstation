using System;
using System.Collections.Generic;
using System.Text;
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
                    _ = LoadRunDetailAsync(value.Id);
            }
        }

        public AvaloniaList<Models.GitHubJobDetail> JobDetails { get; } = [];

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
            catch (Exception ex)
            {
                App.RaiseException(string.Empty, $"Failed to load pipelines: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task RerunWorkflowAsync(Models.GitHubWorkflowRun run)
        {
            if (_client == null || run == null)
                return;

            var (success, error) = await _client.RerunWorkflowAsync(run.Id);
            if (!success)
                App.RaiseException(string.Empty, $"Failed to re-run workflow: {error}");
            else
                await RefreshAsync();
        }

        public void OpenInBrowser(Models.GitHubWorkflowRun run)
        {
            if (!string.IsNullOrEmpty(run?.HtmlUrl))
                Native.OS.OpenBrowser(run.HtmlUrl);
        }

        private async Task LoadRunDetailAsync(long runId)
        {
            if (_client == null)
                return;

            var jobs = await _client.GetJobsAsync(runId);
            var details = new List<Models.GitHubJobDetail>();

            // Fetch logs for all jobs in parallel
            var logTasks = new List<Task<string>>();
            foreach (var job in jobs)
                logTasks.Add(_client.GetJobLogsAsync(job.Id));

            await Task.WhenAll(logTasks);

            for (int i = 0; i < jobs.Count; i++)
            {
                var job = jobs[i];
                var rawLog = logTasks[i].Result;
                var stepLogs = ParseLogsByStep(job.Steps, rawLog);

                details.Add(new Models.GitHubJobDetail
                {
                    Job = job,
                    StepLogs = stepLogs,
                });
            }

            Dispatcher.UIThread.Invoke(() =>
            {
                JobDetails.Clear();
                JobDetails.AddRange(details);
            });
        }

        private static List<Models.GitHubStepLog> ParseLogsByStep(List<Models.GitHubStep> steps, string rawLog)
        {
            var result = new List<Models.GitHubStepLog>();
            if (steps == null)
                return result;

            if (string.IsNullOrEmpty(rawLog))
            {
                foreach (var step in steps)
                    result.Add(new Models.GitHubStepLog { Step = step });
                return result;
            }

            var lines = rawLog.Split('\n');

            // Find step boundaries by matching ##[group] lines to step names in order
            var boundaries = new List<int>();
            int nextStepToFind = 0;

            for (int i = 0; i < lines.Length && nextStepToFind < steps.Count; i++)
            {
                var cleaned = StripTimestamp(lines[i]);

                if (cleaned.StartsWith("##[group]"))
                {
                    var groupName = cleaned[9..].Trim();
                    if (nextStepToFind < steps.Count &&
                        groupName.Contains(steps[nextStepToFind].Name, StringComparison.OrdinalIgnoreCase))
                    {
                        boundaries.Add(i);
                        nextStepToFind++;
                        continue;
                    }
                }

                // First step often has no ##[group] marker
                if (nextStepToFind == 0 && i == 0)
                {
                    boundaries.Add(0);
                    nextStepToFind++;
                }
            }

            for (int s = 0; s < steps.Count; s++)
            {
                var stepLog = new Models.GitHubStepLog { Step = steps[s] };

                if (s < boundaries.Count)
                {
                    int start = boundaries[s];
                    int end = (s + 1 < boundaries.Count) ? boundaries[s + 1] : lines.Length;

                    var sb = new StringBuilder();
                    for (int i = start; i < end; i++)
                    {
                        var line = StripTimestamp(lines[i]);

                        if (line.StartsWith("##[group]"))
                            line = line[9..];
                        else if (line.StartsWith("##[endgroup]"))
                            continue;
                        else if (line.StartsWith("##[section]"))
                            line = line[11..];
                        else if (line.StartsWith("##[command]"))
                            line = "$ " + line[11..];
                        else if (line.StartsWith("##[error]"))
                            line = "ERROR: " + line[9..];
                        else if (line.StartsWith("##[warning]"))
                            line = "WARN: " + line[11..];

                        sb.AppendLine(line);
                    }

                    stepLog.Log = sb.ToString().TrimEnd();
                }

                result.Add(stepLog);
            }

            return result;
        }

        private static string StripTimestamp(string line)
        {
            if (line.Length > 20)
            {
                var spaceIdx = line.IndexOf(' ');
                if (spaceIdx >= 20 && spaceIdx <= 35 && line[spaceIdx - 1] == 'Z')
                    return line[(spaceIdx + 1)..];
            }
            return line;
        }

        private void InitializeClient()
        {
            _client = Models.GitHubClient.TryCreate(_repo.Remotes);
            if (_client != null)
                OnPropertyChanged(nameof(IsGitHubRepo));
        }

        private readonly Repository _repo;
        private Models.GitHubClient _client;
        private Models.GitHubWorkflowRun _selectedRun;
        private bool _isLoading;
    }
}
