using System;

namespace Blog.Models
{
    public class CommentaryEditModel
    {
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public DateTime Time { get; set; }
        public string Reason { get; set; }
    }
}
