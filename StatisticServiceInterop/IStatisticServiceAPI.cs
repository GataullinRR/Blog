using System.Threading.Tasks;

namespace StatisticServiceExports
{
    public interface IStatisticServiceAPI
    {
        Task OnPostActionAsync(PostNotification info);
        Task OnCommentaryActionAsunc(CommentaryNotification info);
        Task OnSeenAsync(SeenNotification info);
        Task OnUserActionAsync(UserNotification info);
        Task OnEntityResolvedAsync(EntityResolvedNotification info);
    }
}
