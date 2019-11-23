using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public class Commentary : IDbEntity, IReportable
    {
        [Key] public int Id { get; set; }
        [Required] public virtual User Author { get; set; }
        public DateTime CreationTime { get; set; }
        [Required] public virtual Post Post { get; set; }
        [Required] public string Body { get; set; }
        [Required] public virtual List<CommentaryEdit> Edits { get; set; } = new List<CommentaryEdit>();
        [InverseProperty(nameof(Report.CommentaryObject))]
        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();
        [InverseProperty(nameof(Violation.CommentaryObject))]
        [Required] public virtual List<Violation> Violations { get; set; } = new List<Violation>();
        [Required] public virtual ViewStatistic ViewStatistic { get; set; }
        public bool IsHidden { get; set; }
        public bool IsDeleted { get; set; }

        public Commentary() { }

        public Commentary(User author, DateTime creationTime, Post post, string body)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            CreationTime = creationTime;
            Post = post ?? throw new ArgumentNullException(nameof(post));
            Body = body ?? throw new ArgumentNullException(nameof(body));
            ViewStatistic = new ViewStatistic();
        }
    }
}
