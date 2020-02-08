using DBModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class PostModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public DateTime CreationTime { get; set; }
        public string Author { get; set; }
        public string AuthorBiography { get; set; }
        public string AuthorId { get; set; }
        public bool IsDeleted { get; set; }
        public string DeleteReason { get; set; }
        public ModerationState ModerationState { get; set; }
        public ProfileImageModel AuthorProfileImage { get; set; }

        public PostEditModel[] Edits { get; set; }
        public CommentarySectionModel CommentarySectionModel { get; set; }

        #region ### USER-SPECIFIC ###

        public bool CanEdit { get; set; }
        public bool CanReport { get; set; }
        public bool CanReportViolation { get; set; }
        public bool CanMarkAsModerated { get; set; }
        public bool CanMarkAsNotPassedModeration { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRestore { get; set; }
        public bool CanAddCommentary { get; set; }
        public bool IsAuthentificated { get; set; }
        public int TotalViews { get; set; }

        #endregion
    }
}
