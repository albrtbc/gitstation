using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SourceGit.Models
{
    public class GitHubClient : IDisposable
    {
        public string Owner { get; }
        public string Repo { get; }

        public GitHubClient(string token, string owner, string repo)
        {
            Owner = owner;
            Repo = repo;
            _client = new HttpClient();
            _client.BaseAddress = new Uri("https://api.github.com");
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            _client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GitStation", "1.0"));
            if (!string.IsNullOrEmpty(token))
                _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("token", token);
            _client.Timeout = TimeSpan.FromSeconds(15);
        }

        /// <summary>
        /// Auto-resolve GitHub token: tries gh CLI first, then falls back to Preferences.
        /// Result is cached after the first call.
        /// </summary>
        public static string ResolveToken()
        {
            if (_tokenResolved)
                return _cachedToken;

            _tokenResolved = true;
            _cachedToken = TryGetGhCliToken();
            if (!string.IsNullOrEmpty(_cachedToken))
                return _cachedToken;

            _cachedToken = ViewModels.Preferences.Instance.GitHubToken;
            return _cachedToken;
        }

        private static string _cachedToken;
        private static bool _tokenResolved;

        public static GitHubClient TryCreate(IReadOnlyList<Remote> remotes)
        {
            var token = ResolveToken();
            if (string.IsNullOrEmpty(token) || remotes == null)
                return null;

            foreach (var remote in remotes)
            {
                if (remote.URL.Contains("github.com", StringComparison.OrdinalIgnoreCase))
                {
                    var (owner, repo) = ParseRemoteUrl(remote.URL);
                    if (!string.IsNullOrEmpty(owner) && !string.IsNullOrEmpty(repo))
                        return new GitHubClient(token, owner, repo);
                }
            }

            return null;
        }

        private static string TryGetGhCliToken()
        {
            // Try gh.exe on Windows PATH
            var candidates = new[] { "gh", "gh.exe" };
            foreach (var exe in candidates)
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = exe,
                        Arguments = "auth token",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    };
                    using var proc = Process.Start(psi);
                    if (proc == null)
                        continue;
                    var output = proc.StandardOutput.ReadToEnd().Trim();
                    proc.WaitForExit();
                    if (proc.ExitCode == 0 && !string.IsNullOrEmpty(output))
                        return output;
                }
                catch { }
            }

            // Try via WSL if on Windows
            if (OperatingSystem.IsWindows())
            {
                try
                {
                    var psi = new ProcessStartInfo
                    {
                        FileName = "wsl",
                        Arguments = "gh auth token",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                    };
                    using var proc = Process.Start(psi);
                    if (proc != null)
                    {
                        var output = proc.StandardOutput.ReadToEnd().Trim();
                        proc.WaitForExit();
                        if (proc.ExitCode == 0 && !string.IsNullOrEmpty(output))
                            return output;
                    }
                }
                catch { }
            }

            return null;
        }

        // Pull Requests
        public async Task<List<GitHubPullRequest>> GetPullRequestsAsync(string state = "open")
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/pulls?state={state}&per_page=50");
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.ListGitHubPullRequest) : [];
        }

        public async Task<GitHubPullRequest> GetPullRequestAsync(int number)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/pulls/{number}");
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubPullRequest) : null;
        }

        public async Task<GitHubPullRequest> CreatePullRequestAsync(string title, string body, string head, string baseBranch)
        {
            var payload = JsonSerializer.Serialize(new CreatePullRequestPayload { Title = title, Body = body, Head = head, Base = baseBranch }, GitHubJsonContext.Default.CreatePullRequestPayload);
            var json = await PostAsync($"/repos/{Owner}/{Repo}/pulls", payload);
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubPullRequest) : null;
        }

        public async Task<bool> UpdatePullRequestAsync(int number, string title, string body)
        {
            var payload = JsonSerializer.Serialize(new UpdatePullRequestPayload { Title = title, Body = body }, GitHubJsonContext.Default.UpdatePullRequestPayload);
            return await PatchAsync($"/repos/{Owner}/{Repo}/pulls/{number}", payload);
        }

        public async Task<(bool success, string error)> MergePullRequestAsync(int number, string mergeMethod)
        {
            var payload = JsonSerializer.Serialize(new MergePullRequestPayload { MergeMethod = mergeMethod }, GitHubJsonContext.Default.MergePullRequestPayload);
            try
            {
                var content = new StringContent(payload, Encoding.UTF8, "application/json");
                var response = await _client.PutAsync($"/repos/{Owner}/{Repo}/pulls/{number}/merge", content);
                var body = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return (true, null);
                return (false, $"{response.StatusCode}: {body}");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        // Comments
        public async Task<List<GitHubComment>> GetCommentsAsync(int prNumber)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/issues/{prNumber}/comments?per_page=100");
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.ListGitHubComment) : [];
        }

        public async Task<GitHubComment> AddCommentAsync(int prNumber, string body)
        {
            var payload = JsonSerializer.Serialize(new CommentPayload { Body = body }, GitHubJsonContext.Default.CommentPayload);
            var json = await PostAsync($"/repos/{Owner}/{Repo}/issues/{prNumber}/comments", payload);
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubComment) : null;
        }

        // Reviews
        public async Task<List<GitHubReview>> GetReviewsAsync(int prNumber)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/pulls/{prNumber}/reviews?per_page=100");
            return json != null ? JsonSerializer.Deserialize(json, GitHubJsonContext.Default.ListGitHubReview) : [];
        }

        public async Task<(bool success, string error)> SubmitReviewAsync(int prNumber, string eventType, string body)
        {
            var payload = JsonSerializer.Serialize(new SubmitReviewPayload { Event = eventType, Body = body }, GitHubJsonContext.Default.SubmitReviewPayload);
            return await PostWithErrorAsync($"/repos/{Owner}/{Repo}/pulls/{prNumber}/reviews", payload);
        }

        public async Task<bool> CreateReviewCommentAsync(int prNumber, string body, string commitId, string path, int line)
        {
            var payload = JsonSerializer.Serialize(new ReviewCommentPayload { Body = body, CommitId = commitId, Path = path, Line = line, Side = "RIGHT" }, GitHubJsonContext.Default.ReviewCommentPayload);
            var json = await PostAsync($"/repos/{Owner}/{Repo}/pulls/{prNumber}/comments", payload);
            return json != null;
        }

        // Check Runs (CI status for a commit)
        public async Task<List<GitHubCheckRun>> GetCheckRunsAsync(string commitSha)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/commits/{commitSha}/check-runs?per_page=100");
            if (json == null)
                return [];
            var response = JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubCheckRunsResponse);
            return response?.CheckRuns ?? [];
        }

        // Workflow Runs (Pipelines)
        public async Task<List<GitHubWorkflowRun>> GetWorkflowRunsAsync(int perPage = 30)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/actions/runs?per_page={perPage}");
            if (json == null)
                return [];
            var response = JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubWorkflowRunsResponse);
            return response?.WorkflowRuns ?? [];
        }

        public async Task<List<GitHubJob>> GetJobsAsync(long runId)
        {
            var json = await GetAsync($"/repos/{Owner}/{Repo}/actions/runs/{runId}/jobs");
            if (json == null)
                return [];
            var response = JsonSerializer.Deserialize(json, GitHubJsonContext.Default.GitHubJobsResponse);
            return response?.Jobs ?? [];
        }

        public async Task<string> GetJobLogsAsync(long jobId)
        {
            try
            {
                var response = await _client.GetAsync($"/repos/{Owner}/{Repo}/actions/jobs/{jobId}/logs");
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            catch { }
            return string.Empty;
        }

        public async Task<(bool success, string error)> RerunWorkflowAsync(long runId)
        {
            return await PostWithErrorAsync($"/repos/{Owner}/{Repo}/actions/runs/{runId}/rerun", "{}");
        }

        // Helper to extract owner/repo from a git remote URL
        public static (string owner, string repo) ParseRemoteUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return (null, null);

            // HTTPS: https://github.com/owner/repo.git
            if (url.StartsWith("https://", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
            {
                try
                {
                    var uri = new Uri(url);
                    var segments = uri.AbsolutePath.Trim('/').Split('/');
                    if (segments.Length >= 2)
                    {
                        var repo = segments[1];
                        if (repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                            repo = repo[..^4];
                        return (segments[0], repo);
                    }
                }
                catch { }
            }

            // SSH: git@github.com:owner/repo.git
            if (url.Contains('@') && url.Contains(':'))
            {
                var colonIdx = url.IndexOf(':');
                var path = url[(colonIdx + 1)..];
                var segments = path.Split('/');
                if (segments.Length >= 2)
                {
                    var repo = segments[1];
                    if (repo.EndsWith(".git", StringComparison.OrdinalIgnoreCase))
                        repo = repo[..^4];
                    return (segments[0], repo);
                }
            }

            return (null, null);
        }

        public void Dispose()
        {
            _client?.Dispose();
        }

        private async Task<string> GetAsync(string path)
        {
            try
            {
                var response = await _client.GetAsync(path);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            catch { }
            return null;
        }

        private async Task<(bool success, string error)> PostWithErrorAsync(string path, string jsonPayload)
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(path, content);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                    return (true, null);
                return (false, $"{response.StatusCode}: {responseBody}");
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }

        private async Task<string> PostAsync(string path, string jsonPayload)
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var response = await _client.PostAsync(path, content);
                if (response.IsSuccessStatusCode)
                    return await response.Content.ReadAsStringAsync();
            }
            catch { }
            return null;
        }

        private async Task<bool> PatchAsync(string path, string jsonPayload)
        {
            try
            {
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Patch, path) { Content = content };
                var response = await _client.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch { }
            return false;
        }

        private readonly HttpClient _client;
    }
}
