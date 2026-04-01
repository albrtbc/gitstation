using System;
using System.Threading;
using System.Threading.Tasks;

namespace SourceGit.Models
{
    public interface IAIService
    {
        string Name { get; }
        string GenerateSubjectPrompt { get; }

        Task ChatAsync(string prompt, string question, CancellationToken cancellation, Action<string> onUpdate);
    }
}
