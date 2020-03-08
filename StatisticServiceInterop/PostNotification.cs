using System;

namespace StatisticServiceExports
{
    [Serializable]
    public class PostNotification
    {
        public int PostId { get; set; }
        public PostAction PostAction { get; set; }

        public PostNotification(int postId, PostAction postAction)
        {
            PostId = postId;
            PostAction = postAction;
        }
    }

    public enum PostAction
    {
        CREATED,
        DELETED 
    }
}
