using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.Models
{
    public class ClaudeCodeService : ObservableObject, IAIService
    {
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string CliPath
        {
            get => _cliPath;
            set => SetProperty(ref _cliPath, value);
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

        public ClaudeCodeService()
        {
            AnalyzeDiffPrompt = """
                You are an expert developer specialist in creating commits.
                Provide a super concise one sentence overall changes summary of the user `git diff` output following strictly the next rules:
                - Do not use any code snippets, imports, file routes or bullets points.
                - Do not mention the route of file that has been change.
                - Write clear, concise, and descriptive messages that explain the MAIN GOAL made of the changes.
                - Use the present tense and active voice in the message, for example, "Fix bug" instead of "Fixed bug.".
                - Use the imperative mood, which gives the message a sense of command, e.g. "Add feature" instead of "Added feature".
                - Avoid using general terms like "update" or "change", be specific about what was updated or changed.
                - Avoid using terms like "The main goal of", just output directly the summary in plain text
                """;

            GenerateSubjectPrompt = """
                You are an expert developer specialist in creating commits messages.
                Based on the provided git diff, generate exactly ONE commit message line.
                Rules:
                - Format: {type}: {message}
                - Types: feat, fix, docs, style, test, chore, revert, refactor
                - Maximum 50 characters total
                - Use present tense imperative mood (e.g. "add", "fix", not "added", "fixed")
                - Output ONLY the commit message, nothing else
                """;
        }

        public async Task ChatAsync(string prompt, string question, CancellationToken cancellation, Action<string> onUpdate)
        {
            var exe = string.IsNullOrWhiteSpace(_cliPath) ? "claude" : _cliPath;
            var combinedPrompt = $"{prompt}\n\n{question}";

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
            start.ArgumentList.Add(combinedPrompt);
            start.ArgumentList.Add("--output-format");
            start.ArgumentList.Add("text");

            var process = new Process() { StartInfo = start };

            try
            {
                process.Start();
                process.StandardInput.Close();

                var rsp = new OpenAIResponse(onUpdate);
                var buffer = new char[512];

                while (!cancellation.IsCancellationRequested)
                {
                    var count = await process.StandardOutput.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellation).ConfigureAwait(false);
                    if (count == 0)
                        break;

                    rsp.Append(new string(buffer, 0, count));
                }

                rsp.End();

                if (cancellation.IsCancellationRequested && !process.HasExited)
                {
                    process.Kill();
                }

                await process.WaitForExitAsync(CancellationToken.None).ConfigureAwait(false);
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
        private string _cliPath = string.Empty;
        private string _analyzeDiffPrompt;
        private string _generateSubjectPrompt;
    }
}
