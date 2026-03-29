using System.Collections.Generic;
using Avalonia.Media;

namespace SourceGit.Models
{
    public class ThemePreset
    {
        public string Name { get; set; }
        public bool IsDark { get; set; }
        public Dictionary<string, Color> Colors { get; set; }

        public override string ToString() => Name;
    }

    public static class ThemePresets
    {
        public static readonly List<ThemePreset> All =
        [
            new() { Name = "Default", IsDark = false, Colors = null },
            new() { Name = "Dark", IsDark = true, Colors = null },
            new() { Name = "Light", IsDark = false, Colors = null },
            new() { Name = "GitLab Dark", IsDark = true, Colors = GitLabDark() },
            new() { Name = "GitLab Light", IsDark = false, Colors = GitLabLight() },
            new() { Name = "Dracula", IsDark = true, Colors = Dracula() },
            new() { Name = "Nord", IsDark = true, Colors = Nord() },
            new() { Name = "Monokai", IsDark = true, Colors = Monokai() },
        ];

        public static ThemePreset Find(string name)
        {
            return All.Find(p => p.Name == name) ?? All[0];
        }

        private static Dictionary<string, Color> GitLabDark() => new()
        {
            ["Color.Window"] = Color.Parse("#FF1A1A2E"),
            ["Color.WindowBorder"] = Color.Parse("#FF3D3D5C"),
            ["Color.TitleBar"] = Color.Parse("#FF16162B"),
            ["Color.ToolBar"] = Color.Parse("#FF1E1E38"),
            ["Color.Popup"] = Color.Parse("#FF222240"),
            ["Color.Contents"] = Color.Parse("#FF141428"),
            ["Color.Badge"] = Color.Parse("#FF6E49CB"),
            ["Color.BadgeFG"] = Color.Parse("#FFFFFFFF"),
            ["Color.Conflict"] = Color.Parse("#FFFCA326"),
            ["Color.Conflict.Foreground"] = Color.Parse("#FF1A1A2E"),
            ["Color.Border0"] = Color.Parse("#FF2A2A4A"),
            ["Color.Border1"] = Color.Parse("#FF5A5A8A"),
            ["Color.Border2"] = Color.Parse("#FF3A3A5E"),
            ["Color.FlatButton.Background"] = Color.Parse("#FF252545"),
            ["Color.FlatButton.BackgroundHovered"] = Color.Parse("#FF2D2D50"),
            ["Color.FlatButton.FloatingBorder"] = Color.Parse("#FF4A4A70"),
            ["Color.FG1"] = Color.Parse("#FFE1E1E6"),
            ["Color.FG2"] = Color.Parse("#FF9696B4"),
            ["Color.Diff.EmptyBG"] = Color.Parse("#3C000000"),
            ["Color.Diff.AddedBG"] = Color.Parse("#C0245830"),
            ["Color.Diff.DeletedBG"] = Color.Parse("#C0702428"),
            ["Color.Diff.AddedHighlight"] = Color.Parse("#A01B8C37"),
            ["Color.Diff.DeletedHighlight"] = Color.Parse("#A0A8333A"),
            ["Color.Diff.BlockBorderHighlight"] = Color.Parse("#FF6E49CB"),
            ["Color.Link"] = Color.Parse("#FF6E94F2"),
            ["Color.InlineCode"] = Color.Parse("#FF2A2A4A"),
            ["Color.InlineCodeFG"] = Color.Parse("#FFE1E1E6"),
            ["Color.DataGridHeaderBG"] = Color.Parse("#FF1E1E38"),
            ["SystemAccentColor"] = Color.Parse("#FF6E49CB"),
        };

        private static Dictionary<string, Color> GitLabLight() => new()
        {
            ["Color.Window"] = Color.Parse("#FFFAFAFA"),
            ["Color.WindowBorder"] = Color.Parse("#FFDBDBDB"),
            ["Color.TitleBar"] = Color.Parse("#FFF0F0F0"),
            ["Color.ToolBar"] = Color.Parse("#FFF5F5F5"),
            ["Color.Popup"] = Color.Parse("#FFFFFFFF"),
            ["Color.Contents"] = Color.Parse("#FFFFFFFF"),
            ["Color.Badge"] = Color.Parse("#FF7B58CF"),
            ["Color.BadgeFG"] = Color.Parse("#FFFFFFFF"),
            ["Color.Conflict"] = Color.Parse("#FFE9A019"),
            ["Color.Conflict.Foreground"] = Color.Parse("#FFFFFFFF"),
            ["Color.Border0"] = Color.Parse("#FFDBDBDB"),
            ["Color.Border1"] = Color.Parse("#FFA0A0A0"),
            ["Color.Border2"] = Color.Parse("#FFE0E0E0"),
            ["Color.FlatButton.Background"] = Color.Parse("#FFF0F0F0"),
            ["Color.FlatButton.BackgroundHovered"] = Color.Parse("#FFFFFFFF"),
            ["Color.FlatButton.FloatingBorder"] = Color.Parse("#FFA0A0A0"),
            ["Color.FG1"] = Color.Parse("#FF303030"),
            ["Color.FG2"] = Color.Parse("#FF737373"),
            ["Color.Diff.EmptyBG"] = Color.Parse("#10000000"),
            ["Color.Diff.AddedBG"] = Color.Parse("#80C3E6C3"),
            ["Color.Diff.DeletedBG"] = Color.Parse("#80F0B4B4"),
            ["Color.Diff.AddedHighlight"] = Color.Parse("#A7E1A7"),
            ["Color.Diff.DeletedHighlight"] = Color.Parse("#F19B9D"),
            ["Color.Diff.BlockBorderHighlight"] = Color.Parse("#FF7B58CF"),
            ["Color.Link"] = Color.Parse("#FF1068BF"),
            ["Color.InlineCode"] = Color.Parse("#FFECEBF0"),
            ["Color.InlineCodeFG"] = Color.Parse("#FF303030"),
            ["Color.DataGridHeaderBG"] = Color.Parse("#FFF5F5F5"),
            ["SystemAccentColor"] = Color.Parse("#FF7B58CF"),
        };

        private static Dictionary<string, Color> Dracula() => new()
        {
            ["Color.Window"] = Color.Parse("#FF282A36"),
            ["Color.WindowBorder"] = Color.Parse("#FF44475A"),
            ["Color.TitleBar"] = Color.Parse("#FF21222C"),
            ["Color.ToolBar"] = Color.Parse("#FF2D2F3D"),
            ["Color.Popup"] = Color.Parse("#FF343746"),
            ["Color.Contents"] = Color.Parse("#FF1E1F29"),
            ["Color.Badge"] = Color.Parse("#FFBD93F9"),
            ["Color.BadgeFG"] = Color.Parse("#FF282A36"),
            ["Color.Conflict"] = Color.Parse("#FFFFB86C"),
            ["Color.Conflict.Foreground"] = Color.Parse("#FF282A36"),
            ["Color.Border0"] = Color.Parse("#FF191A21"),
            ["Color.Border1"] = Color.Parse("#FF6272A4"),
            ["Color.Border2"] = Color.Parse("#FF44475A"),
            ["Color.FlatButton.Background"] = Color.Parse("#FF343746"),
            ["Color.FlatButton.BackgroundHovered"] = Color.Parse("#FF3C3F58"),
            ["Color.FlatButton.FloatingBorder"] = Color.Parse("#FF44475A"),
            ["Color.FG1"] = Color.Parse("#FFF8F8F2"),
            ["Color.FG2"] = Color.Parse("#FF6272A4"),
            ["Color.Diff.EmptyBG"] = Color.Parse("#3C000000"),
            ["Color.Diff.AddedBG"] = Color.Parse("#C02D4F35"),
            ["Color.Diff.DeletedBG"] = Color.Parse("#C05E2D3B"),
            ["Color.Diff.AddedHighlight"] = Color.Parse("#A050FA7B"),
            ["Color.Diff.DeletedHighlight"] = Color.Parse("#A0FF5555"),
            ["Color.Diff.BlockBorderHighlight"] = Color.Parse("#FFBD93F9"),
            ["Color.Link"] = Color.Parse("#FF8BE9FD"),
            ["Color.InlineCode"] = Color.Parse("#FF44475A"),
            ["Color.InlineCodeFG"] = Color.Parse("#FFF8F8F2"),
            ["Color.DataGridHeaderBG"] = Color.Parse("#FF2D2F3D"),
            ["SystemAccentColor"] = Color.Parse("#FFBD93F9"),
        };

        private static Dictionary<string, Color> Nord() => new()
        {
            ["Color.Window"] = Color.Parse("#FF2E3440"),
            ["Color.WindowBorder"] = Color.Parse("#FF4C566A"),
            ["Color.TitleBar"] = Color.Parse("#FF272C36"),
            ["Color.ToolBar"] = Color.Parse("#FF353B49"),
            ["Color.Popup"] = Color.Parse("#FF3B4252"),
            ["Color.Contents"] = Color.Parse("#FF292E3A"),
            ["Color.Badge"] = Color.Parse("#FF5E81AC"),
            ["Color.BadgeFG"] = Color.Parse("#FFECEFF4"),
            ["Color.Conflict"] = Color.Parse("#FFEBCB8B"),
            ["Color.Conflict.Foreground"] = Color.Parse("#FF2E3440"),
            ["Color.Border0"] = Color.Parse("#FF232831"),
            ["Color.Border1"] = Color.Parse("#FF616E88"),
            ["Color.Border2"] = Color.Parse("#FF434C5E"),
            ["Color.FlatButton.Background"] = Color.Parse("#FF3B4252"),
            ["Color.FlatButton.BackgroundHovered"] = Color.Parse("#FF434C5E"),
            ["Color.FlatButton.FloatingBorder"] = Color.Parse("#FF4C566A"),
            ["Color.FG1"] = Color.Parse("#FFD8DEE9"),
            ["Color.FG2"] = Color.Parse("#FF7B88A1"),
            ["Color.Diff.EmptyBG"] = Color.Parse("#3C000000"),
            ["Color.Diff.AddedBG"] = Color.Parse("#C02D4A37"),
            ["Color.Diff.DeletedBG"] = Color.Parse("#C0593A3E"),
            ["Color.Diff.AddedHighlight"] = Color.Parse("#A0A3BE8C"),
            ["Color.Diff.DeletedHighlight"] = Color.Parse("#A0BF616A"),
            ["Color.Diff.BlockBorderHighlight"] = Color.Parse("#FF88C0D0"),
            ["Color.Link"] = Color.Parse("#FF88C0D0"),
            ["Color.InlineCode"] = Color.Parse("#FF3B4252"),
            ["Color.InlineCodeFG"] = Color.Parse("#FFECEFF4"),
            ["Color.DataGridHeaderBG"] = Color.Parse("#FF353B49"),
            ["SystemAccentColor"] = Color.Parse("#FF5E81AC"),
        };

        private static Dictionary<string, Color> Monokai() => new()
        {
            ["Color.Window"] = Color.Parse("#FF272822"),
            ["Color.WindowBorder"] = Color.Parse("#FF49483E"),
            ["Color.TitleBar"] = Color.Parse("#FF1E1F1C"),
            ["Color.ToolBar"] = Color.Parse("#FF2D2E27"),
            ["Color.Popup"] = Color.Parse("#FF3E3D32"),
            ["Color.Contents"] = Color.Parse("#FF1E1F1C"),
            ["Color.Badge"] = Color.Parse("#FFA6E22E"),
            ["Color.BadgeFG"] = Color.Parse("#FF272822"),
            ["Color.Conflict"] = Color.Parse("#FFE6DB74"),
            ["Color.Conflict.Foreground"] = Color.Parse("#FF272822"),
            ["Color.Border0"] = Color.Parse("#FF1A1B16"),
            ["Color.Border1"] = Color.Parse("#FF75715E"),
            ["Color.Border2"] = Color.Parse("#FF49483E"),
            ["Color.FlatButton.Background"] = Color.Parse("#FF3E3D32"),
            ["Color.FlatButton.BackgroundHovered"] = Color.Parse("#FF49483E"),
            ["Color.FlatButton.FloatingBorder"] = Color.Parse("#FF49483E"),
            ["Color.FG1"] = Color.Parse("#FFF8F8F2"),
            ["Color.FG2"] = Color.Parse("#FF75715E"),
            ["Color.Diff.EmptyBG"] = Color.Parse("#3C000000"),
            ["Color.Diff.AddedBG"] = Color.Parse("#C03A5C25"),
            ["Color.Diff.DeletedBG"] = Color.Parse("#C06E2B2D"),
            ["Color.Diff.AddedHighlight"] = Color.Parse("#A0A6E22E"),
            ["Color.Diff.DeletedHighlight"] = Color.Parse("#A0F92672"),
            ["Color.Diff.BlockBorderHighlight"] = Color.Parse("#FF66D9EF"),
            ["Color.Link"] = Color.Parse("#FF66D9EF"),
            ["Color.InlineCode"] = Color.Parse("#FF3E3D32"),
            ["Color.InlineCodeFG"] = Color.Parse("#FFF8F8F2"),
            ["Color.DataGridHeaderBG"] = Color.Parse("#FF2D2E27"),
            ["SystemAccentColor"] = Color.Parse("#FFA6E22E"),
        };
    }
}
