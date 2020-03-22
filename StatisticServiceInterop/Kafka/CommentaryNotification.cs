using System;

namespace StatisticServiceExports.Kafka
{
    [Serializable]
    public class CommentaryNotification
    {
        public int CommentaryId { get; set; }
        public CommentaryAction Action { get; set; }

        public CommentaryNotification(int commentaryId, CommentaryAction action)
        {
            CommentaryId = commentaryId;
            Action = action;
        }
    }

    public enum CommentaryAction
    {
        CREATED,
        DELETED
    }
}
