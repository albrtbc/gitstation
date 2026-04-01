using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SourceGit.Commands
{
    /// <summary>
    ///     A C# version of https://github.com/anjerodev/commitollama
    /// </summary>
    public class GenerateCommitMessage
    {
        public class GetDiffContent : Command
        {
            public GetDiffContent(string repo, Models.DiffOption opt)
            {
                WorkingDirectory = repo;
                Context = repo;
                Args = $"diff --no-color --no-ext-diff --diff-algorithm=minimal {opt}";
            }

            public async Task<Result> ReadAsync()
            {
                return await ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public GenerateCommitMessage(Models.IAIService service, string repo, List<Models.Change> changes, CancellationToken cancelToken, Action<string> onResponse)
        {
            _service = service;
            _repo = repo;
            _changes = changes;
            _cancelToken = cancelToken;
            _onResponse = onResponse;
        }

        public async Task ExecAsync()
        {
            try
            {
                _onResponse?.Invoke("Collecting diffs...");

                var diffBuilder = new StringBuilder();
                foreach (var change in _changes)
                {
                    if (_cancelToken.IsCancellationRequested)
                        return;

                    var rs = await new GetDiffContent(_repo, new Models.DiffOption(change, false)).ReadAsync();
                    if (rs.IsSuccess)
                        diffBuilder.Append(rs.StdOut);
                }

                if (_cancelToken.IsCancellationRequested)
                    return;

                // Phase 1: Analyze all diffs into a purpose-focused summary
                _onResponse?.Invoke("Analyzing changes...");

                var summaryBuilder = new StringBuilder();
                await _service.ChatAsync(
                    _service.AnalyzeDiffPrompt,
                    $"Here is the `git diff` output:\n{diffBuilder}",
                    _cancelToken,
                    update => summaryBuilder.Append(update));

                if (_cancelToken.IsCancellationRequested)
                    return;

                // Phase 2: Generate commit message from the summary
                _onResponse?.Invoke("Generating commit message...");

                var resultBuilder = new StringBuilder();
                await _service.ChatAsync(
                    _service.GenerateSubjectPrompt,
                    $"Here are the summary of changes:\n{summaryBuilder}",
                    _cancelToken,
                    update =>
                    {
                        resultBuilder.Append(update);
                        _onResponse?.Invoke(resultBuilder.ToString().Trim());
                    });
            }
            catch (Exception e)
            {
                App.RaiseException(_repo, $"Failed to generate commit message: {e}");
            }
        }

        private Models.IAIService _service;
        private string _repo;
        private List<Models.Change> _changes;
        private CancellationToken _cancelToken;
        private Action<string> _onResponse;
    }
}
