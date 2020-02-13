using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Middlewares;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;
using Microsoft.EntityFrameworkCore;
using Blog.Attributes;
using Blog.Middlewares.CachingMiddleware.Policies;

namespace Blog.Controllers
{
    public class AdminPanelTabsController : ControllerBase
    {
        class PostsTableRowModel
        {
            public string Post { get; set; }
            public string Author { get; set; }
            public long CreatTime { get; set; }
            public int ViewsCnt { get; set; }
            public int ComCnt { get; set; }
            public double RepPView { get; set; }
        }

        public static readonly string FULL_POSTS_TABLE_URI = getURIToAction(nameof(AdminPanelTabsController), nameof(LoadFullPostsTableAsync));

        public AdminPanelTabsController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [ServerResponseCache(24 * 3600, CachePolicy.AUTHORIZED_USER_SCOPED)]
        public async Task<IActionResult> LoadOverviewTabAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            S.Db.ChangeTracker.LazyLoadingEnabled = false;
            var daysStatistic = S.Db.Blogs
                .AsNoTracking()
                .Include(b => b.Statistic)
                .ThenInclude(s => s.DayStatistics)
                .ThenInclude(s => s.PostsViewStatistic)
                .Include(b => b.Statistic)
                .ThenInclude(s => s.DayStatistics)
                .ThenInclude(s => s.CommentariesViewStatistic)
                .Single().Statistic.DayStatistics
                .OrderBy(ds => ds.Day)
                .ToArray();

            return PartialView("AdminPanel/_OverviewTab", daysStatistic);
        }

        [ServerResponseCache(3600 * 24, CachePolicy.AUTHORIZED_USER_SCOPED)]
        public async Task<IActionResult> LoadFullPostsTableAsync()
        {
            S.Db.ChangeTracker.LazyLoadingEnabled = false;
            var result = S.Db.Posts
                .AsNoTracking()
                .Select(post =>
                    new PostsTableRowModel()
                    {
                        Author = $"{post.Author.Id}|{post.Author.UserName}",
                        ComCnt = post.Commentaries.Count(),
                        Post = $"{post.Id}|{post.Title.Take(30).Aggregate()}...",
                        CreatTime = new DateTimeOffset(post.CreationTime).ToUnixTimeSeconds(),
                        ViewsCnt = post.ViewStatistic.TotalViews,
                        RepPView = post.ViewStatistic.TotalViews == 0 
                        ? 0
                        : (double)post.Reports.Count / post.ViewStatistic.TotalViews 
                        // This line is same, but causes runtime exception
                        //((double)post.Reports.Count / (double)post.ViewStatistic.TotalViews).Exchange(double.NaN, 0D)
                    })
                .ToArray();

            return Json(result);
        }

#warning Refactor! Hadn't had time to do better!
        [ServerResponseCache(3600 * 24, CachePolicy.AUTHORIZED_USER_SCOPED)]
        public async Task<IActionResult> LoadModeratorsTabAsync()
        {
            S.Db.ChangeTracker.LazyLoadingEnabled = false;
            var groups = S.Db.ModeratorsGroups
                .AsNoTracking()
                .OrderBy(mg => mg.CreationTime)
                .Include(mg => mg.Statistic)
                .ThenInclude(s => s.DayStatistics)
                .Include(mg => mg.PostEditsToCheck)
                .Include(mg => mg.CommentariesToCheck)
                .Include(mg => mg.ProfilesToCheck)
                .Include(mg => mg.PostsToCheck)
                .Include(mg => mg.Moderators)
                .ToArray();
            var startDate = groups[0].CreationTime.Date;
            var endDate = DateTime.UtcNow.Date;
            var xAxis = ((endDate - startDate).TotalDays + 1)
                .Round()
                .Range()
                .Select(dayOffset => startDate.AddDays(dayOffset))
                .ToArray();

            var i = 0;
            var groupsInfos = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo[groups.Length];
            foreach (var group in groups)
            {
                var moderatorsInfos = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo[group.Moderators.Count()];
                var statistic = await group.Statistic.DayStatistics
                    .ToArrayAsync();
                var resolvedEntities = new int[xAxis.Length];
                var resolveTime = new double[xAxis.Length];
                for (int j = 0; j < xAxis.Length; j++)
                {
                    var day = xAxis[j];
                    var dayStatistic = statistic.FirstOrDefault(s => s.Day == day) ?? new ModeratorsGroupDayStatistic();
                    resolvedEntities[j] = dayStatistic.ResolvedEntitiesCount;
                    resolveTime[j] = dayStatistic.SummedResolveTime != default
                        ? dayStatistic.SummedResolveTime.TotalSeconds.Round() 
                        : double.NaN;
                }

#warning add moderator statistic entity!
                var m = 0;
                var moderators = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo[group.Moderators.Count];
                foreach (var moderator in group.Moderators)
                {
                    var resolvedByModeratorEntities = group.EntitiesToCheck
                        .Where(e => e.IsResolved && e.AssignedModerator == moderator)
                        .GroupBy(e => e.ResolvingTime.Value.Date)
                        .ToArray();
                    var reslovedByModeratorEntitiesCounts = new int[xAxis.Length];
                    var moderatorResolveTime = new double[xAxis.Length];
                    moderatorResolveTime.SetAll(double.NaN);
                    for (int j = 0; j < resolvedByModeratorEntities.Length; j++)
                    {
                        var entities = resolvedByModeratorEntities[j];
                        var l = (entities.Key - startDate).TotalDays.Round();
                        reslovedByModeratorEntitiesCounts[l] = entities.Count();
                        moderatorResolveTime[l] = entities.Select(e => (e.ResolvingTime.Value - e.AddTime).TotalSeconds).Sum();
                    }

                    moderators[m++] = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo(moderator.UserName, null, resolvedEntities, moderatorResolveTime, null);
                }

                groupsInfos[i++] = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo(
                    "Group" + i,
                    group.Id,
                    resolvedEntities,
                    resolveTime,
                    moderators);
            }

            var vm = new AdminPanelModeratorsTabModel(new AdminPanelModeratorsTabModel.SummaryModel(xAxis, groupsInfos));
            return PartialView("AdminPanel/_ModeratorsTab", vm);
        }
    }
}
