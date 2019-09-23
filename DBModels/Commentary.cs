using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class Commentary
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public User Author { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public Post Post { get; set; }
        [Required]
        public string Body { get; set; }
    }
}
