using Avalonia.Controls;

namespace SourceGit.Views
{
    public partial class AIPRReview : ChromelessWindow
    {
        public AIPRReview()
        {
            CloseOnESC = true;
            InitializeComponent();
        }

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            (DataContext as ViewModels.AIPRReview)?.Cancel();
        }
    }
}
