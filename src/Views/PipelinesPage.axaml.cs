using Avalonia.Controls;
using Avalonia.Interactivity;

namespace SourceGit.Views
{
    public partial class PipelinesPage : UserControl
    {
        public PipelinesPage()
        {
            InitializeComponent();
        }

        protected override async void OnLoaded(RoutedEventArgs e)
        {
            base.OnLoaded(e);
            if (DataContext is ViewModels.PipelinesPage vm)
                await vm.RefreshAsync();
        }

        private async void OnRefresh(object s, RoutedEventArgs e)
        {
            if (DataContext is ViewModels.PipelinesPage vm)
                await vm.RefreshAsync();
        }
    }
}
