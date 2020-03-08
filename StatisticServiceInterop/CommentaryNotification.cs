using System;

namespace StatisticServiceExports
{
    [Serializable]
    public class CommentaryNotification
    {
        public CommentaryNotification(int commentaryId, CommentaryAction action)
        {
            CommentaryId = commentaryId;
            Action = action;
        }

        public int CommentaryId { get; set; }
        public CommentaryAction Action { get; set; }
    }

    public enum CommentaryAction
    {
        CREATED,
        DELETED,
        SEEN,
        SEEN_BY_REGISTERED_USER,
    }
}
