using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class PostEdit : EditBase, IModeratable
    {
        [Required] public string NewTitle { get; set; }
        [Required] public string NewBody { get; set; }
        [Required] public string NewBodyPreview { get; set; }
        public virtual Post Post { get; set; }
        public ModerationState State { get; set; }

        public PostEdit() { }

        public PostEdit(User editAuthor, string reason, DateTime editTime, string newTitle, string newBody, string newBodyPreview)
        {
            Author = editAuthor ?? throw new ArgumentNullException(nameof(editAuthor));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            EditTime = editTime;
            NewBody = newBody ?? throw new ArgumentNullException(nameof(reason));
            NewTitle = newTitle ?? throw new ArgumentNullException(nameof(reason));
            NewBodyPreview = newBodyPreview ?? throw new ArgumentNullException(nameof(reason));
        }
    }
}
