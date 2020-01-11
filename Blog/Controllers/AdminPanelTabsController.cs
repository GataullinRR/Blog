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

            var daysStatistic = await S.Db.Blog.Single().Statistic.DayStatistics
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
    }
}
