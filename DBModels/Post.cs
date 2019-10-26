using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModels
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public DateTime CreationTime { get; set; }
        [Required]
        public virtual User Author { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Body { get; set; }
        [Required]
        public virtual List<PostEdit> Edits { get; set; } = new List<PostEdit>();
        [Required] public virtual List<PostReport> Reports { get; set; } = new List<PostReport>();

        public Post() { }

        public Post(DateTime creationTime, User author, string title, string body)
        {
            CreationTime = creationTime;
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Body = body ?? throw new ArgumentNullException(nameof(body));
        }
    }
}
