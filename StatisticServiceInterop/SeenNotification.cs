using System;
using System.Collections.Generic;

namespace StatisticServiceExports
{
    [Serializable]
    public class SeenNotification
    {
        public bool IsSeenByRegisteredUser { get; set; }
        /// <summary>
        /// [PostId, SeenTimes (delta)]
        /// </summary>
        public Dictionary<int, int> SeenPosts { get; set; }
        public Dictionary<int, int> SeenCommentaries { get; set; }
        public Dictionary<int, int> SeenProfiles { get; set; }

        public SeenNotification(bool isSeenByRegisteredUser)
        {
            IsSeenByRegisteredUser = isSeenByRegisteredUser;
        }
    }
}
