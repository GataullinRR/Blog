using Blog.Attributes;
using Blog.Misc;
using DBModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;
using Utilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SINGLETON)]
    public class DbEntitiesUpdateService
    {
        class ViewStatisticDelta
        {
            public int TotalViews;
            public int RegisteredUserViews;
        }

        readonly ConcurrentDictionary<int, ViewStatisticDelta> _postsDeltas = new ConcurrentDictionary<int, ViewStatisticDelta>();
        readonly ConcurrentDictionary<int, ViewStatisticDelta> _commentariesDeltas = new ConcurrentDictionary<int, ViewStatisticDelta>();
        readonly ConcurrentDictionary<int, ViewStatisticDelta> _profilesDeltas = new ConcurrentDictionary<int, ViewStatisticDelta>();
        
        readonly ILogger _logger;
        readonly IServiceScopeFactory _scopeFactory;

        public DbEntitiesUpdateService(ILoggerFactory loggerFactory, IServiceScopeFactory scopeFactory)
        {
            _logger = loggerFactory.CreateLogger<DbEntitiesUpdateService>();
            _scopeFactory = scopeFactory;
            
            loopAsync();
        }

        public async Task UpdateViewStatisticAsync(bool isUserRegisteredUser, IViewStatistic viewStatistic)
        {
            updateViewStatistic(isUserRegisteredUser, (dynamic)viewStatistic);
        }
        void updateViewStatistic(bool isUserRegisteredUser, IViewStatistic<Post> viewStatistic)
        {
            var statistic = _postsDeltas.GetOrAdd(viewStatistic.Id, new ViewStatisticDelta());
            increment(isUserRegisteredUser, statistic);
        }
        void updateViewStatistic(bool isUserRegisteredUser, IViewStatistic<Commentary> viewStatistic)
        {
            var statistic = _commentariesDeltas.GetOrAdd(viewStatistic.Id, new ViewStatisticDelta());
            increment(isUserRegisteredUser, statistic);
        }
        void updateViewStatistic(bool isUserRegisteredUser, IViewStatistic<Profile> viewStatistic)
        {
            var statistic = _profilesDeltas.GetOrAdd(viewStatistic.Id, new ViewStatisticDelta());
            increment(isUserRegisteredUser, statistic);
        }
        void increment(bool isUserRegisteredUser, ViewStatisticDelta statistic)
        {
            if (isUserRegisteredUser)
            {
                Interlocked.Increment(ref statistic.RegisteredUserViews);
            }
            Interlocked.Increment(ref statistic.TotalViews);
        }

        async void loopAsync()
        {
            while (true)
            {
                await Task.Delay(5 * 60 * 1000);

                try
                {
                    using (var scope = _scopeFactory.CreateScope())
                    using (var db = scope.ServiceProvider.GetService<BlogContext>())
                    {
                        update(_postsDeltas, db.PostViews);
                        update(_commentariesDeltas, db.CommentaryViews);
                        update(_profilesDeltas, db.ProfileViews);

                        await db.SaveChangesAsync();
                    }

                    _logger.LogDebug("View statistic has been updated");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Could not commit view statistic deltas");
                }
            }

            void update(ConcurrentDictionary<int, ViewStatisticDelta> deltas, IQueryable<IViewStatistic> dbCollection)
            {
                var ids = deltas.Keys.ToArray();
                var dbStatistics = dbCollection
                    .Where(s => ids.Contains(s.Id))
                    .ToArray();
                for (int i = 0; i < dbStatistics.Length; i++)
                {
                    var dbStatistic = dbStatistics[i];
                    var d = deltas[dbStatistic.Id];
                    dbStatistics[i].RegisteredUserViews += d.RegisteredUserViews;
                    dbStatistics[i].TotalViews += d.TotalViews;
                    _postsDeltas.TryRemove(dbStatistic.Id, out _);
                }
            }
        }
    }
}
