using System;

namespace Blog.Models
{
    public class ViolationModel
    {
        public DateTime CreationTime { get; set; }
        public object Object { get; set; }
        public string Description { get; set; }
        public string Reporter { get; set; }
        public string ReporterId { get; set; }
    }
}
