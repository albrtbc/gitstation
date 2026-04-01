using System.Threading;
using System.Threading.Tasks;

using Avalonia.Threading;

using CommunityToolkit.Mvvm.ComponentModel;

namespace SourceGit.ViewModels
{
    public class AIPRReview : ObservableObject
    {
        public bool IsGenerating
        {
            get => _isGenerating;
            private set => SetProperty(ref _isGenerating, value);
        }

        public string Text
        {
            get => _text;
            private set => SetProperty(ref _text, value);
        }

        public AIPRReview(Repository repo, Models.IAIService service, string baseRef, string headRef)
        {
            _repo = repo;
            _service = service;
            _baseRef = baseRef;
            _headRef = headRef;
            _cancel = new CancellationTokenSource();

            Gen();
        }

        public void Regen()
        {
            if (_cancel is { IsCancellationRequested: false })
                _cancel.Cancel();

            Gen();
        }

        public void Cancel()
        {
            _cancel?.Cancel();
        }

        private void Gen()
        {
            Text = string.Empty;
            IsGenerating = true;

            _cancel = new CancellationTokenSource();
            Task.Run(async () =>
            {
                await new Commands.GeneratePRReview(_service, _repo.FullPath, _baseRef, _headRef, _cancel.Token, message =>
                {
                    Dispatcher.UIThread.Post(() => Text = message);
                }).ExecAsync().ConfigureAwait(false);

                Dispatcher.UIThread.Post(() => IsGenerating = false);
            }, _cancel.Token);
        }

        private readonly Repository _repo = null;
        private Models.IAIService _service = null;
        private string _baseRef;
        private string _headRef;
        private CancellationTokenSource _cancel = null;
        private bool _isGenerating = false;
        private string _text = string.Empty;
    }
}
