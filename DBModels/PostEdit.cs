using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class PostEdit : EditBase
    {
        [Required] public string OldTitle { get; set; }
        [Required] public string OldBody { get; set; }
        [Required] public string OldBodyPreview { get; set; }
        public bool MadeWhilePublished { get; set; }
        public virtual Post Post { get; set; }

        public PostEdit() { }

        public PostEdit(User editAuthor, string reason, DateTime editTime, Post oldPost)
        {
            Author = editAuthor ?? throw new ArgumentNullException(nameof(editAuthor));
            Reason = reason ?? throw new ArgumentNullException(nameof(reason));
            EditTime = editTime;
            OldBody = oldPost.Body;
            OldTitle = oldPost.Title;
            OldBodyPreview = oldPost.BodyPreview;
            MadeWhilePublished = oldPost.ModerationInfo.State == ModerationState.MODERATED;
        }
    }
}
