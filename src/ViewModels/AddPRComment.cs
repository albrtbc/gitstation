using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SourceGit.ViewModels
{
    public class AddPRComment : Popup
    {
        [Required(ErrorMessage = "Comment is required")]
        public string Comment
        {
            get => _comment;
            set => SetProperty(ref _comment, value, true);
        }

        public string FilePath { get; }
        public int Line { get; }

        public AddPRComment(PullRequestsPage prPage, string filePath, int line)
        {
            _prPage = prPage;
            FilePath = filePath;
            Line = line;
        }

        public override async Task<bool> Sure()
        {
            if (string.IsNullOrWhiteSpace(_comment))
                return false;

            ProgressDescription = "Posting comment...";
            var success = await _prPage.AddInlineCommentAsync(FilePath, Line, _comment);
            if (!success)
                App.RaiseException(string.Empty, "Failed to post comment.");
            return success;
        }

        private readonly PullRequestsPage _prPage;
        private string _comment = string.Empty;
    }
}
