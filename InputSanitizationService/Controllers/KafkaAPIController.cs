using ASPCoreUtilities.Types;
using StatisticServiceExports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StatisticService.Controllers
{
    /// <summary>
    /// Methods in this "controller" are being called when Kafka messages arrive.
    /// </summary>
    [Service(ServiceType.SCOPED, typeof(IStatisticServiceAPI))]
    public class KafkaAPIController : IStatisticServiceAPI
    {
        public async Task OnCommentaryActionAsunc(CommentaryNotification info)
        {
        }

        public async Task OnEntityResolvedAsync(EntityResolvedNotification info)
        {
        }

        public async Task OnPostActionAsync(PostNotification info)
        {
        }

        public async Task OnSeenAsync(SeenNotification info)
        {
        }

        public async Task OnUserActionAsync(UserNotification info)
        {
        }
    }
}
