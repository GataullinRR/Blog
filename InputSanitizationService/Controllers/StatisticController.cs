using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using CommonDBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using StatisticDBModels;
using StatisticServiceExports;
using Utilities;

namespace InputSanitizationService.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class StatisticController : ControllerBase
    {
        readonly BlogStatisticContext _db;

        public StatisticController()
        {

        }

        [HttpPost(Name = "UpdateActionStatistic")]
        public async Task UpdateActionsStatistic([Required]ActionPerformedInfo actionInfo)
        {

        }

        [HttpPost(Name = "UpdateActionStatistic")]
        public async Task CommentaryCreated([Required]ActionPerformedInfo actionInfo)
        {

        }

        [HttpPost(Name = "UpdateActionStatistic")]
        public async Task CommentaryCreated([Required]ActionPerformedInfo actionInfo)
        {

        }
    }

    //[ApiController]
    //[Route("/api/[controller]")]
    //public class StatisticController : ControllerBase
    //{
    //    readonly BlogStatisticContext _db;

    //    public StatisticController()
    //    {

    //    }

    //    [HttpPost(Name = "UpdateActionStatistic")]
    //    public async Task UpdateActionsStatistic([Required]ActionPerformedDTO actionInfo)
    //    {
    //        var statistic = await _db.UserStatistics
    //            .Include(us => us.ActionsTotal)
    //            .FirstOrDefaultAsync(us => us.UserId == actionInfo.PerformerId);
    //        if (statistic == null)
    //        {
    //            statistic = new UserStatistic(actionInfo.PerformerId);
    //            _db.UserStatistics.Add(statistic);
    //        }

    //        var action = statistic.ActionsTotal.FirstOrDefault(a => a.ActionType == actionInfo.ActionType);
    //        if (action == null)
    //        {
    //            action = new ActionStatistic<UserStatistic>(action.ActionType, 0);
    //            statistic.ActionsTotal.Add(action);
    //        }

    //        action.Count++;

    //        await _db.SaveChangesAsync();
    //    }

    //    [HttpPost(Name = "UpdateActionStatistic")]
    //    public async Task UpdateActionsStatistic([Required]EntityResolvedDTO entityResolved)
    //    {
    //        var statistic = await _db.ModeratorsGroupStatistics
    //            .FirstOrDefaultAsync(ms => ms.OwnerId == entityResolved.ModeratorGroupId);
    //        if (statistic == null)
    //        {
    //            statistic = new ModeratorsGroupStatistic();
    //            _db.ModeratorsGroupStatistics.Add(statistic);
    //        }

    //        static

    //        var action = statistic.ActionsTotal.FirstOrDefault(a => a.ActionType == actionInfo.ActionType);
    //        if (action == null)
    //        {
    //            action = new ActionStatistic<UserStatistic>(action.ActionType, 0);
    //            statistic.ActionsTotal.Add(action);
    //        }

    //        action.Count++;

    //        await _db.SaveChangesAsync();
    //    }

    //    //async Task updateUserActionsStatistic(ActionPerformedDTO actionInfo)
    //    //{
    //    //    var statistic = await ensureHasThisDayUserStatisticAsync(actionInfo.PerformerId);
    //    //    ensureHasAppropriateCounter().Count++;

    //    //    IActionStatistic ensureHasAppropriateCounter()
    //    //    {
    //    //        ActionStatistic<UserDayStatistic> actionCounter;
    //    //        if (statistic.Actions.Any(s => s.ActionType == actionInfo.ActionType))
    //    //        {
    //    //            actionCounter = statistic.Actions.FirstOrDefault(s => s.ActionType == actionInfo.ActionType);
    //    //        }
    //    //        else // Executes only in case of new ActionType had been added
    //    //        {
    //    //            actionCounter = new ActionStatistic<UserDayStatistic>()
    //    //            {
    //    //                ActionType = actionInfo.ActionType,
    //    //                 DayStatistic = statistic,

    //    //            };
    //    //            statistic.Actions.Add(actionCounter);
    //    //        }

    //    //        return actionCounter;
    //    //    }
    //    //}

    //    //async Task<UserDayStatistic> ensureHasThisDayUserStatisticAsync(string userId)
    //    //{
    //    //    var today = DateTime.UtcNow.Date;
    //    //    var dayStatistic = await _db.UserDayStatistics
    //    //        .Where(ds => ds.Statistic.UserId == userId && ds.Day == today)
    //    //        .Include(ds => ds.Actions)
    //    //        .FirstOrDefaultAsync();
    //    //    if (dayStatistic == null)
    //    //    {
    //    //        dayStatistic = new UserDayStatistic()
    //    //        {
    //    //            Day = today,
    //    //            Statistic = await ensureHasUserStatisticAsync(),
    //    //            Actions = await getActionsAsync()
    //    //        };
    //    //        _db.UserDayStatistics.Add(dayStatistic);
    //    //    }

    //    //    return dayStatistic;

    //    //    async Task<ICollection<ActionStatistic<UserDayStatistic>>> getActionsAsync()
    //    //    {
    //    //        var previousDayStatistic = await _db.UserDayStatistics
    //    //            .Where(ds => ds.Statistic.UserId == userId)
    //    //            .OrderByDescending(ds => ds.Day)
    //    //            .Include(ds => ds.Actions)
    //    //            .AsNoTracking()
    //    //            .FirstOrDefaultAsync();
    //    //        if (previousDayStatistic?.Day >= today)
    //    //        {
    //    //            throw new InvalidOperationException("Incorrect time");
    //    //        }

    //    //        var actions = previousDayStatistic?.Actions 
    //    //            ?? EnumUtils.GetValues<ActionType>()
    //    //               .Select(a => new ActionStatistic<UserDayStatistic>(a, 0, 0))
    //    //               .ToArray();
    //    //        foreach (var action in actions)
    //    //        {
    //    //            action.Count = 0;
    //    //        }

    //    //        return actions;
    //    //    }

    //    //    async Task<UserStatistic> ensureHasUserStatisticAsync()
    //    //    {
    //    //        var statistic = await _db.UserStatistics.FirstOrDefaultAsync(us => us.UserId == userId);
    //    //        if (statistic == null)
    //    //        {
    //    //            statistic = new UserStatistic(userId);
    //    //            _db.UserStatistics.Add(statistic);
    //    //        }

    //    //        return statistic;
    //    //    }
    //    //}

    //    [HttpPost(Name = "UpdatePostsAndCommentaries")]
    //    public async Task UpdateProfileViewStatisticAsync(
    //        [Required]bool isSeenByRegistered,
    //        [Required]int[] profileIds)
    //    {
    //        await tryUpdateStatistic(isSeenByRegistered, _db.ProfileViewStatistics, profileIds);
    //        await _db.SaveChangesAsync();
    //    }

    //    [HttpPost(Name = "UpdatePostsAndCommentaries")]
    //    public async Task UpdateViewStatisticAsync(
    //        [Required]bool isSeenByRegistered, 
    //        int[] postIds, 
    //        int[] commentaryIds)
    //    {
    //        await tryUpdateStatistic(isSeenByRegistered, _db.PostViewStatistics, postIds);
    //        await tryUpdateStatistic(isSeenByRegistered, _db.CommentaryViewStatistics, commentaryIds);
    //        await _db.SaveChangesAsync();
    //    }

    //    async Task tryUpdateStatistic(bool isSeenByRegistered, DbSet<ViewStatistic<int>> viewStatistics, int[] ids)
    //    {
    //        if (ids?.Length > 0)
    //        {
    //            var statistics = await fetchExistingAsync();
    //            await ensureViewStatisticsCreatedAsync();
    //            update();

    //            return; ////////////////////////////

    //            async Task<List<ViewStatistic<int>>> fetchExistingAsync()
    //            {
    //                return await viewStatistics
    //                    .Where(vs => ids.Contains(vs.Id))
    //                    .ToListAsync();
    //            }

    //            async Task ensureViewStatisticsCreatedAsync()
    //            {
    //                var dCount = ids.Length - statistics.Count;
    //                if (dCount > 0) // Some statistics do not exists
    //                {
    //                    var existing = statistics
    //                        .Select(s => s.Id)
    //                        .ToArray();
    //                    var shouldBeAdded = ids
    //                        .Where(id => !existing.Contains(id))
    //                        .Select(id => new ViewStatistic<int>(id))
    //                        .ToArray();
    //                    await viewStatistics.AddRangeAsync(shouldBeAdded);
    //                    statistics.AddRange(shouldBeAdded);
    //                }
    //            }

    //            void update()
    //            {
    //                for (int i = 0; i < statistics.Count; i++)
    //                {
    //                    var stat = statistics[i];
    //                    stat.TotalViews++;
    //                    stat.RegisteredUserViews += isSeenByRegistered ? 1 : 0;
    //                }
    //            }
    //        }
    //    }
    //}
}
