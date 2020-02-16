using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
#if TESTING
    public class TestingController : ControllerBase
    {
        public TestingController(ServiceLocator serviceProvider) : base(serviceProvider)
        {
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<JsonResult> GetAllPostsAsync()
        {
            var urls = new List<string>();
            foreach (var post in S.Db.Posts.Where(p => !p.IsDeleted && p.ModerationInfo.State == DBModels.ModerationState.MODERATED))
            {
                urls.Add(this.Url.Page("/Post", "Get", new { id = post.Id }, this.Request.Scheme));
            }

            return new JsonResult(urls);
        }
    }
#endif
}