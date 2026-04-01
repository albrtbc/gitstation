using System;
using System.Threading;
using System.Threading.Tasks;

namespace SourceGit.Models
{
    public interface IAIService
    {
        const string DefaultGenerateSubjectPrompt = """
            You are an expert developer specialist in creating commits messages.
            Based on the provided git diff, generate exactly ONE commit message line.
            Rules:
            - Format: {type}: {message}
            - Types: feat, fix, docs, style, test, chore, revert, refactor
            - Maximum 50 characters total
            - Use present tense imperative mood (e.g. "add", "fix", not "added", "fixed")
            - Output ONLY the commit message, nothing else
            """;

        string Name { get; }
        string GenerateSubjectPrompt { get; }

        Task ChatAsync(string prompt, string question, CancellationToken cancellation, Action<string> onUpdate);
    }
}
