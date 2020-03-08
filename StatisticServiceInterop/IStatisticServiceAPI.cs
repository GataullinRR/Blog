namespace StatisticServiceExports
{
    public interface IStatisticServiceAPI
    {
        void OnPostAction(PostNotification info);
        void OnCommentaryAction(CommentaryNotification info);
        void OnSeen(SeenNotification info);
        void OnUserAction(UserNotification info);
        void OnEntityResolved(EntityResolvedNotification info);
    }
}
