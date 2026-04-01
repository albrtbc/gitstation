using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SourceGit.Commands
{
    public class GeneratePRReview
    {
        public class GetPRDiff : Command
        {
            public GetPRDiff(string repo, string baseRef, string headRef)
            {
                WorkingDirectory = repo;
                Context = repo;
                Args = $"diff --no-color --no-ext-diff --diff-algorithm=minimal {baseRef}..{headRef}";
            }

            public async Task<Result> ReadAsync()
            {
                return await ReadToEndAsync().ConfigureAwait(false);
            }
        }

        public GeneratePRReview(Models.IAIService service, string repo, string baseRef, string headRef, CancellationToken cancelToken, Action<string> onResponse)
        {
            _service = service;
            _repo = repo;
            _baseRef = baseRef;
            _headRef = headRef;
            _cancelToken = cancelToken;
            _onResponse = onResponse;
        }

        public async Task ExecAsync()
        {
            try
            {
                _onResponse?.Invoke("Fetching PR diff...");

                var rs = await new GetPRDiff(_repo, _baseRef, _headRef).ReadAsync();
                if (!rs.IsSuccess || string.IsNullOrWhiteSpace(rs.StdOut) || _cancelToken.IsCancellationRequested)
                    return;

                _onResponse?.Invoke("Reviewing changes...");

                var resultBuilder = new StringBuilder();
                await _service.ChatAsync(
                    _service.ReviewPRPrompt,
                    $"Here is the pull request diff to review:\n{rs.StdOut}",
                    _cancelToken,
                    update =>
                    {
                        resultBuilder.Append(update);
                        _onResponse?.Invoke(resultBuilder.ToString());
                    });

                _onResponse?.Invoke(resultBuilder.ToString().Trim());
            }
            catch (Exception e)
            {
                App.RaiseException(_repo, $"Failed to generate PR review: {e}");
            }
        }

        private Models.IAIService _service;
        private string _repo;
        private string _baseRef;
        private string _headRef;
        private CancellationToken _cancelToken;
        private Action<string> _onResponse;
    }
}
