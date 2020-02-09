using System;

namespace Blog.Models
{
    public class PostIndexModel
    {
        public int PostId { get; set; }
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public string Title { get; set; }
        public string BodyPreview { get; set; }
        public DateTime CreationTime { get; set; }
    }
}
