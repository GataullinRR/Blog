using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Middlewares;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public class AdminPanelTabsController : ControllerBase
    {
        class PostsTableRowModel
        {
            public string PostId { get; set; }
            public string Author { get; set; }
            public int ViewsCnt { get; set; }
            public int ComCnt { get; set; }
            public double RepPView { get; set; }
        }

        public AdminPanelTabsController(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadOverviewTabAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            var daysStatistic = await S.Db.Blog.Statistic.DayStatistics
                .AsQueryable()
                .OrderBy(ds => ds.Day)
                .ToAsyncEnumerable()
                .ToArray();

            return PartialView("AdminPanel/_OverviewTab", daysStatistic);
        }

        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadFullPostsTableAsync()
        {
            var i = 0;
            var result = new PostsTableRowModel[S.Db.Posts.Count()];
            foreach (var post in S.Db.Posts)
            {
                result[i++] = new PostsTableRowModel()
                {
                    Author = post.Author.UserName,
                    ComCnt = post.Commentaries.Count(),
                    PostId = post.Id.ToString(),
                    ViewsCnt = post.ViewStatistic.TotalViews,
                    RepPView = post.Reports.Count / (double)post.ViewStatistic.TotalViews
                };
            };

            return Json(result);
        }

#warning Refactor! Hadn't had time to do better!
        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadModeratorsTabAsync()
        {
            var startDate = S.Db.ModeratorsGroups
                .Min(mg => mg.CreationTime).Date;
            var endDate = DateTime.UtcNow.Date;
            var xAxis = (endDate - startDate).TotalDays
                .Round()
                .Range()
                .Select(dayOffset => startDate.AddDays(dayOffset))
                .ToArray();

            var i = 0;
            var groupsInfos = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo[S.Db.ModeratorsGroups.Count()];
            foreach (var group in S.Db.ModeratorsGroups.OrderBy(mg => mg.CreationTime))
            {
                var moderatorsInfos = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo[group.Moderators.Count()];
                var statistic = await group.Statistic.DayStatistics
                    .ToArrayAsync();
                var resolvedEntities = new int[xAxis.Length];
                var resolveTime = new double[xAxis.Length];
                for (int j = 0; j < statistic.Length; j++)
                {
                    var dayStatistic = statistic[j];
                    var k = (dayStatistic.Day - startDate).TotalDays.Round();
                    resolvedEntities[k] = dayStatistic.ResolvedEntitiesCount;
                    resolveTime[k] = dayStatistic.SummedResolveTime != default
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

                    moderators[m++] = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo(moderator.UserName, resolvedEntities, moderatorResolveTime, null);
                }

                groupsInfos[i++] = new AdminPanelModeratorsTabModel.SummaryModel.GroupInfo(
                    "Group" + i,
                    resolvedEntities,
                    resolveTime,
                    moderators);
            }

            var vm = new AdminPanelModeratorsTabModel(new AdminPanelModeratorsTabModel.SummaryModel(xAxis, groupsInfos));
            return PartialView("AdminPanel/_ModeratorsTab", vm);
        }
    }
}
