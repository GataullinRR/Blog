using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DBModels
{
    public class Post : IDbEntity, IReportable, IModeratable, IDeletable
    {
        [Key] public int Id { get; set; }
        [Required] public DateTime CreationTime { get; set; }
        [InverseProperty(nameof(User.Posts))]
        [Required] public virtual User Author { get; set; }
        [Required] public string Title { get; set; }
        [Required] public string Body { get; set; }
        [Required] public string BodyPreview { get; set; }
        [Required] public virtual List<PostEdit> Edits { get; set; } = new List<PostEdit>();
        public PostEdit LastEdit => Edits.OrderByDescending(e => e.EditTime).FirstOrDefault();
        [Required] public virtual ViewStatistic ViewStatistic { get; set; }
        [InverseProperty(nameof(Report.PostObject))]
        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();
        [InverseProperty(nameof(Violation.PostObject))]
        [Required] public virtual List<Violation> Violations { get; set; } = new List<Violation>();
        public bool IsDeleted { get; set; }
        [Required] public virtual ModerationInfo ModerationInfo { get; set; }

        public Post() 
        { 

        }

        public Post(DateTime creationTime, User author, string title, string body, string bodyPreview)
        {
            CreationTime = creationTime;
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            BodyPreview = bodyPreview ?? throw new ArgumentNullException(nameof(bodyPreview));
            ViewStatistic = new ViewStatistic();
            ModerationInfo = new ModerationInfo();
        }
    }
}
