using System;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace SourceGit.Converters
{
    public static class DateTimeConverters
    {
        public static readonly FuncValueConverter<DateTime, string> ToRelativeTime =
            new(v => FormatRelative(v));

        public static string FormatRelative(DateTime localTime)
        {
            var now = DateTime.Now;
            var span = now - localTime;

            if (span.TotalMinutes < 1)
                return App.Text("Period.JustNow");
            if (span.TotalHours < 1)
                return App.Text("Period.MinutesAgo", (int)span.TotalMinutes);
            if (span.TotalDays < 1)
            {
                var hours = (int)span.TotalHours;
                return hours == 1 ? App.Text("Period.HourAgo") : App.Text("Period.HoursAgo", hours);
            }

            var lastDay = now.AddDays(-1).Date;
            if (localTime >= lastDay)
                return App.Text("Period.Yesterday");

            if ((localTime.Year == now.Year && localTime.Month == now.Month) || span.TotalDays < 28)
                return App.Text("Period.DaysAgo", (int)(now.Date - localTime.Date).TotalDays);

            var lastMonth = now.AddMonths(-1).Date;
            if (localTime.Year == lastMonth.Year && localTime.Month == lastMonth.Month)
                return App.Text("Period.LastMonth");

            if (localTime.Year == now.Year || localTime > now.AddMonths(-11))
                return App.Text("Period.MonthsAgo", (12 + now.Month - localTime.Month) % 12);

            var diffYear = now.Year - localTime.Year;
            return diffYear == 1 ? App.Text("Period.LastYear") : App.Text("Period.YearsAgo", diffYear);
        }

        public static readonly FuncValueConverter<DateTime?, string> ToNullableRelativeTime =
            new(v => v.HasValue ? ToRelativeTime.Convert(v.Value, typeof(string), null, null) as string : "");
    }

    public static class GitHubConverters
    {
        public static readonly FuncValueConverter<string, IBrush> ConclusionToColor =
            new(v => v switch
            {
                "success" => Brushes.LimeGreen,
                "failure" => Brushes.Red,
                "cancelled" => Brushes.Gray,
                "skipped" => Brushes.Gray,
                "timed_out" => Brushes.Orange,
                "action_required" => Brushes.Orange,
                _ => Brushes.DodgerBlue,
            });

        public static readonly FuncValueConverter<string, string> ConclusionToIcon =
            new(v => v switch
            {
                "success" => "✓",
                "failure" => "✗",
                "cancelled" => "⊘",
                "skipped" => "⊘",
                _ => "●",
            });

        public static readonly FuncValueConverter<string, string> ShortSha =
            new(v => string.IsNullOrEmpty(v) ? "" : v.Length > 7 ? v[..7] : v);
    }
}
