using System;

namespace Blog.Models
{
    public class PostEditModel
    {
        public string Author { get; set; }
        public string AuthorId { get; set; }
        public string Reason { get; set; }
        public DateTime EditTime { get; set; }
    }
}
