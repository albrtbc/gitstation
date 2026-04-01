using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.Models
{
    public class CLIAIService : ObservableObject, IAIService
    {
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string Executable
        {
            get => _executable;
            set => SetProperty(ref _executable, value);
        }

        public string AnalyzeDiffPrompt
        {
            get => _analyzeDiffPrompt;
            set => SetProperty(ref _analyzeDiffPrompt, value);
        }

        public string GenerateSubjectPrompt
        {
            get => _generateSubjectPrompt;
            set => SetProperty(ref _generateSubjectPrompt, value);
        }

        public string ReviewPRPrompt
        {
            get => _reviewPRPrompt;
            set => SetProperty(ref _reviewPRPrompt, value);
        }

        public CLIAIService()
        {
            AnalyzeDiffPrompt = IAIService.DefaultAnalyzeDiffPrompt;
            GenerateSubjectPrompt = IAIService.DefaultGenerateSubjectPrompt;
            ReviewPRPrompt = IAIService.DefaultReviewPRPrompt;
        }

        public async Task ChatAsync(string prompt, string question, CancellationToken cancellation, Action<string> onUpdate)
        {
            var exe = string.IsNullOrWhiteSpace(_executable) ? "claude" : _executable;

            var start = new ProcessStartInfo()
            {
                FileName = exe,
                UseShellExecute = false,
                CreateNoWindow = true,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                StandardOutputEncoding = Encoding.UTF8,
                StandardErrorEncoding = Encoding.UTF8,
            };
            start.ArgumentList.Add("-p");
            start.ArgumentList.Add(prompt);
            start.ArgumentList.Add("--output-format");
            start.ArgumentList.Add("text");

            var process = new Process() { StartInfo = start };

            try
            {
                process.Start();

                await process.StandardInput.WriteAsync(question.AsMemory(), cancellation).ConfigureAwait(false);
                process.StandardInput.Close();

                var stderrTask = process.StandardError.ReadToEndAsync(cancellation);

                var buffer = new char[512];
                var trimmedStart = false;

                while (!cancellation.IsCancellationRequested)
                {
                    var count = await process.StandardOutput.ReadAsync(buffer.AsMemory(), cancellation).ConfigureAwait(false);
                    if (count == 0)
                        break;

                    var text = new string(buffer, 0, count);
                    if (!trimmedStart)
                    {
                        text = text.TrimStart();
                        if (text.Length == 0)
                            continue;
                        trimmedStart = true;
                    }

                    onUpdate?.Invoke(text);
                }

                if (cancellation.IsCancellationRequested && !process.HasExited)
                    process.Kill();

                var stderr = await stderrTask.ConfigureAwait(false);
                await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);

                if (process.ExitCode != 0 && !cancellation.IsCancellationRequested)
                    throw new Exception($"CLI exited with code {process.ExitCode}: {stderr?.Trim()}");
            }
            catch
            {
                if (!cancellation.IsCancellationRequested)
                    throw;
            }
            finally
            {
                process.Dispose();
            }
        }

        private string _name;
        private string _executable = string.Empty;
        private string _analyzeDiffPrompt;
        private string _generateSubjectPrompt;
        private string _reviewPRPrompt;
    }
}
