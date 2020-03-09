using ASPCoreUtilities.Types;
using Microsoft.EntityFrameworkCore;
using StatisticDBModels;
using System;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace StatisticService.Services
{
    public interface IDbAccess
    {
        Task<BlogDayStatistic> GetBlogDayStatisticAsync(DateTime date);
    }

#warning add caching
    [Service(ServiceType.SCOPED, typeof(IDbAccess))]
    class DbAccess : IDbAccess
    {
        readonly StatisticContext _db;

        public DbAccess(StatisticContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public async Task<BlogDayStatistic> GetBlogDayStatisticAsync(DateTime date)
        {
            var currentDayStatistic = await _db.BlogDaysStatistic
                .OrderByDescending(ds => ds.Day)
                .Include(ds => ds.UsersWithStateCount)
                .FirstOrDefaultAsync();
            if (currentDayStatistic == null)
            {
                currentDayStatistic = new BlogDayStatistic(date, null);
                _db.BlogDaysStatistic.Add(currentDayStatistic);
            }
            else if (currentDayStatistic.Day < date)
            {
                var missingDaysCount = (date - currentDayStatistic.Day).TotalDays.Round();
                for (int i = 1; i <= missingDaysCount; i++)
                {
                    var missingDay = currentDayStatistic.Day.AddDays(i);
                    var missingDayStat = new BlogDayStatistic(missingDay, currentDayStatistic);
                    _db.BlogDaysStatistic.Add(missingDayStat);

                    if (i == missingDaysCount)
                    {
                        currentDayStatistic = missingDayStat;
                    }
                }
            }
            else if (currentDayStatistic.Day > date)
            {
                throw new InvalidOperationException("Time is not synchronized");
            }

            return currentDayStatistic;
        }
    }
}
