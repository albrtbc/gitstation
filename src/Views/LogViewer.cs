using Avalonia;
using Avalonia.Media;

using AvaloniaEdit;

namespace SourceGit.Views
{
    public class LogViewer : TextEditor
    {
        public static readonly StyledProperty<string> LogTextProperty =
            AvaloniaProperty.Register<LogViewer, string>(nameof(LogText));

        public string LogText
        {
            get => GetValue(LogTextProperty);
            set => SetValue(LogTextProperty, value);
        }

        public LogViewer()
        {
            IsReadOnly = true;
            ShowLineNumbers = false;
            WordWrap = false;
            HorizontalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
            VerticalScrollBarVisibility = Avalonia.Controls.Primitives.ScrollBarVisibility.Auto;
            FontFamily = new FontFamily("fonts:GitStation#JetBrains Mono");
            FontSize = 11;
            Background = Brushes.Transparent;
            BorderThickness = new Thickness(0);
            Padding = new Thickness(16, 4);
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == LogTextProperty)
            {
                Text = change.GetNewValue<string>() ?? string.Empty;
            }
        }
    }
}
