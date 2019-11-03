using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public class Report : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, InverseProperty(nameof(User.ReportedReports))]
        public virtual User Reporter { get; set; }
        //[InverseProperty(nameof(User.ReportedReports)]
        public virtual User ReportObjectOwner { get; set; }
        public virtual DateTime CreationDate { get; set; }
        [InverseProperty(nameof(Commentary.Reports))]
        public virtual Commentary CommentaryObject { get; set; }
        [InverseProperty(nameof(Post.Reports))]
        public virtual Post PostObject { get; set; }
        [InverseProperty(nameof(Profile.Reports))]
        public virtual Profile ProfileObject { get; set; }

        public IReportObject ReportObject => CommentaryObject ?? (IReportObject)ProfileObject ?? PostObject;

        public Report() { }

        public Report(User reporter, User reportObjectOwner, object reportObject)
        {
            Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            ReportObjectOwner = reportObjectOwner ?? throw new ArgumentNullException(nameof(reportObjectOwner));
            CreationDate = DateTime.UtcNow;
            CommentaryObject = reportObject as Commentary;
            PostObject = reportObject as Post;
            ProfileObject = reportObject as Profile;
        }
    }
}
