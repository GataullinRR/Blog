using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public abstract class ReportBase : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual User Reporter { get; set; }
        public virtual User ObjectOwner { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public virtual Commentary CommentaryObject { get; set; }
        public virtual Post PostObject { get; set; }
        public virtual Profile ProfileObject { get; set; }

        public IModeratableObject Object => CommentaryObject ?? (IModeratableObject)ProfileObject ?? PostObject;

        public ReportBase() { }

        public ReportBase(User reporter, User reportObjectOwner, object reportObject)
        {
            Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            ObjectOwner = reportObjectOwner ?? throw new ArgumentNullException(nameof(reportObjectOwner));
            CreationDate = DateTime.UtcNow;
            CommentaryObject = reportObject as Commentary;
            PostObject = reportObject as Post;
            ProfileObject = reportObject as Profile;
        }
    }
}
