using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class CommentaryEdit : EditBase
    {
        public CommentaryEdit() { }

        public CommentaryEdit(User editAuthor, string reason, DateTime editTime)
        {
            Author = editAuthor ?? throw new ArgumentNullException(nameof(editAuthor));
            Reason = reason;
            EditTime = editTime;
        }
    }
}
