using System.Threading.Tasks;

namespace StatisticServiceExports
{
    public interface IStatisticServiceAPI
    {
        Task OnPostActionAsync(PostNotification info);
        Task OnCommentaryActionAsync(CommentaryNotification info);
        Task OnSeenAsync(SeenNotification info);
        Task OnUserActionAsync(UserNotification info);
    }
}
