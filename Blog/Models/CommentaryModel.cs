using Blog.Services;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Models
{
    public class CommentaryModel 
    {
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public int CommentaryId { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsHidden { get; set; }
        public string Body { get; set; }
        public IViewStatistic ViewStatistic { get; set; }

        public CommentaryPermissionsModel Permissions { get; set; }
        public CommentaryEditModel[] Edits { get; set; }
        public ProfileImageModel AuthorProfileImage { get; set; }
    }

    public class CommentaryPermissionsModel
    {
        public bool CanEdit { get; set; }
        public bool CanReport { get; set; }
        public bool CanReportViolation { get; set; }
        public bool CanDelete { get; set; }
        public bool CanRestore { get; set; }
    }
}
