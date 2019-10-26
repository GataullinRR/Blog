using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class Commentary
    {
        [Key] public int Id { get; set; }
        [Required] public virtual User Author { get; set; }
        [Required] public DateTime CreationTime { get; set; }
        [Required] public virtual Post Post { get; set; }
        [Required] public string Body { get; set; }
        [Required] public virtual List<CommentaryEdit> Edits { get; set; } = new List<CommentaryEdit>();
        [Required] public virtual List<CommentaryReport> Reports { get; set; } = new List<CommentaryReport>();

        public Commentary() { }

        public Commentary(User author, DateTime creationTime, Post post, string body)
        {
            Author = author ?? throw new ArgumentNullException(nameof(author));
            CreationTime = creationTime;
            Post = post ?? throw new ArgumentNullException(nameof(post));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
    }
}
