using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AdminPanelTabsController : ControllerBase
    {
        public AdminPanelTabsController(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        [ResponseCache(CacheProfileName = ResponseCaching.DAILY)]
        public async Task<IActionResult> LoadOverviewTabAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            var daysStatistic = await S.Db.Blog.Single().Statistic.DayStatistics
                .AsQueryable()
                .OrderBy(ds => ds.Day)
                .ToAsyncEnumerable()
                .ToArray();
            //return Content("Hello");
            return PartialView("AdminPanel/_OverviewTab", daysStatistic);
        }
    }
}