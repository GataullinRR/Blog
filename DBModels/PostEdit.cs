using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class PostEdit
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual User EditAuthor { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime EditTime { get; set; }

        public PostEdit() { }

        public PostEdit(User editAuthor, string reason, DateTime editTime)
        {
            EditAuthor = editAuthor ?? throw new ArgumentNullException(nameof(editAuthor));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            EditTime = editTime;
        }
    }
}
