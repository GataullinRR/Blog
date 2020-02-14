using System;

namespace Blog.Models
{
    public class ReportModel
    {
        public string Reporter { get; set; }
        public string ReporterId { get; set; }
        public DateTime CreationTime { get; set; }
        public object Object { get; set; }
    }
}
