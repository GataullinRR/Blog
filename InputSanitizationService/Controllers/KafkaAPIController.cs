using ASPCoreUtilities.Types;
using Microsoft.EntityFrameworkCore;
using StatisticDBModels;
using StatisticService.Services;
using StatisticServiceExports;
using StatisticServiceExports.Kafka;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace StatisticService.Controllers
{
    /// <summary>
    /// Actions in this "controller" are being called when Kafka messages arrive.
    /// </summary>
    [Service(ServiceType.SCOPED, typeof(IStatisticServiceAPI))]
    public class KafkaAPIController : IStatisticServiceAPI
    {
        readonly StatisticContext _db;
        readonly IDbAccess _dbAccess;

        public KafkaAPIController(StatisticContext db, IDbAccess dbAccess)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _dbAccess = dbAccess ?? throw new ArgumentNullException(nameof(dbAccess));
        }

        public async Task OnCommentaryActionAsync(CommentaryNotification info)
        {
            var currentDayStatistic = await _dbAccess.GetBlogDayStatisticAsync(DateTime.UtcNow.Date);
            var countDelta = info.Action switch
            {
                CommentaryAction.CREATED => 1,
                CommentaryAction.DELETED => -1,
                _ => throw new NotSupportedException()
            };
            currentDayStatistic.CommentariesCount += countDelta;

#warning save changes should not be here
            await _db.SaveChangesAsync();
        }

        public async Task OnPostActionAsync(PostNotification info)
        {
            var currentDayStatistic = await _dbAccess.GetBlogDayStatisticAsync(DateTime.UtcNow.Date);
            var countDelta = info.Action switch
            {
                PostAction.CREATED => 1,
                PostAction.DELETED => -1,
                _ => throw new NotSupportedException()
            };
            currentDayStatistic.PostsCount += countDelta;

            await _db.SaveChangesAsync();
        }

        public async Task OnSeenAsync(SeenNotification info)
        {
            await updateForEntitiesAsync(_db.CommentariesViewStatistic, info.SeenCommentaries);
            await updateForEntitiesAsync(_db.PostsViewStatistic, info.SeenPosts);
            await updateForEntitiesAsync(_db.ProfilesViewStatistic, info.SeenProfiles);

            await _db.SaveChangesAsync();

            return; ////////////////////////////////////////////

            async Task updateForEntitiesAsync<TKey, T>(DbSet<ViewStatistic<TKey, T>> viewStatTable, Dictionary<TKey, int> deltas)
                where TKey : notnull
            {
                var entityIds = deltas.Keys
                    .OrderBy(k => k)
                    .ToArray();
                var entitiesStatsToUpdate = await viewStatTable
                    .Where(vs => entityIds.Contains(vs.Id))
                    .OrderBy(vs => vs.Id)
                    .ToListAsync();
                for (int i = 0; i < entityIds.Length; i++)
                {
                    var cId = entityIds[i];
                    var delta = deltas[cId];
                    var viewStat = entitiesStatsToUpdate[i];
                    update(viewStat, delta);
                }

                void update(IViewStatistic vs, int delta)
                {
                    if (delta < 0)
                    {
                        return;
                    }

                    if (info.IsSeenByRegisteredUser)
                    {
                        vs.RegisteredUserViews += delta;
                    }
                    vs.AllViews += delta;
                }
            }
        }

        public async Task OnUserActionAsync(UserNotification info)
        {
            var currentDayStatistic = await _dbAccess.GetBlogDayStatisticAsync(DateTime.UtcNow.Date);
            var handler = getHandler();
            await handler();
            
            await _db.SaveChangesAsync();

            Func<Task> getHandler() => info switch
            {
                UserNotification(_, null) => async () =>
                {
                    var state = info.Registered.ProfileState;
                    currentDayStatistic.UsersWithStateCount.First(u => u.State == state).Count++;
                },
                UserNotification(null, _) => async () =>
                {
                    currentDayStatistic.UsersWithStateCount.First(u => u.State == info.StateChanged.NewProfileState).Count++;
                    currentDayStatistic.UsersWithStateCount.First(u => u.State == info.StateChanged.OldProfileState).Count--;
                },
                _ => throw new NotSupportedException()
            };
        }
    }
}
