using System;
using System.Text.Json.Serialization;

namespace SourceGit.Models
{
    public class Version
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("tag_name")]
        public string TagName { get; set; }

        [JsonPropertyName("published_at")]
        public DateTime PublishedAt { get; set; }

        [JsonPropertyName("body")]
        public string Body { get; set; }

        [JsonPropertyName("html_url")]
        public string HtmlUrl { get; set; }

        [JsonIgnore]
        public System.Version CurrentVersion
        {
            get
            {
                try
                {
                    return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
                }
                catch
                {
                    return null;
                }
            }
        }

        [JsonIgnore]
        public string CurrentVersionStr => $"v{CurrentVersion?.ToString(3) ?? "0.0.0"}";

        [JsonIgnore]
        public bool IsNewVersion
        {
            get
            {
                try
                {
                    var current = CurrentVersion;
                    if (current == null)
                        return false;

                    var tagVer = TagName;
                    var vIdx = tagVer.LastIndexOf('v');
                    if (vIdx >= 0)
                        tagVer = tagVer[(vIdx + 1)..];

                    var remote = new System.Version(tagVer);
                    return remote > current;
                }
                catch
                {
                    return false;
                }
            }
        }

        [JsonIgnore]
        public string ReleaseDateStr => DateTimeFormat.Format(PublishedAt, true);
    }

    public class AlreadyUpToDate;

    public class SelfUpdateFailed
    {
        public string Reason
        {
            get;
            private set;
        }

        public SelfUpdateFailed(Exception e)
        {
            if (e.InnerException is { } inner)
                Reason = inner.Message;
            else
                Reason = e.Message;
        }
    }
}
