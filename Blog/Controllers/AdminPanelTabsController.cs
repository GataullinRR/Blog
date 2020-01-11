using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Middlewares;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AdminPanelTabsController : ControllerBase
    {
        public AdminPanelTabsController(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadOverviewTabAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            var daysStatistic = await S.Db.Blog.Single().Statistic.DayStatistics
                .AsQueryable()
                .OrderBy(ds => ds.Day)
                .ToAsyncEnumerable()
                .ToArray();

            return PartialView("AdminPanel/_OverviewTab", daysStatistic);
        }

        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadPublicationsTabAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            var daysStatistic = await S.Db.Blog.Single().Statistic.DayStatistics
                .AsQueryable()
                .OrderBy(ds => ds.Day)
                .ToAsyncEnumerable()
                .ToArray();

            return PartialView("AdminPanel/_OverviewTab", daysStatistic);
        }
    }
}