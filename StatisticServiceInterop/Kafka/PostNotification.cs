using System;

namespace StatisticServiceExports.Kafka
{
    [Serializable]
    public class PostNotification
    {
        public int PostId { get; set; }
        public PostAction Action { get; set; }

        public PostNotification(int postId, PostAction postAction)
        {
            PostId = postId;
            Action = postAction;
        }
    }

    public enum PostAction
    {
        CREATED,
        DELETED 
    }
}
