using DBModels;
using System;

namespace Blog.Models
{
    public class PostProfileModel
    {
        public int PostId { get; set; }
        public string Title { get; set; }
        public string BodyPreview { get; set; }
        public DateTime CreationTime { get; set; }
        public bool IsDeleted { get; set; }
        public ModerationState ModerationState { get; set; }
    }
}
