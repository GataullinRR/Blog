using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class PostEditInfo
    {
        public int Id { get; set; }
        [Required]
        public User Author { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime EditTime { get; set; }
    }
}
