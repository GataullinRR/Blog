using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Misc;
using Blog.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utilities.Types;
using Utilities.Extensions;
using Blog.Middlewares;

namespace Blog.Controllers
{
    public class PanelsTableController : ControllerBase
    {
        public PanelsTableController(ServicesLocator serviceProvider) : base(serviceProvider)
        {
        }

        class UsersTableRowModel
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public int ActCnt { get; set; }
            public int PubCnt { get; set; }
            public double RepPPub { get; set; }
        }

        [CustomResponseCache(3600, 3600 * 24, CacheMode.USER_SCOPED)]
        public async Task<IActionResult> LoadFullUsersTableAsync()
        {
            var i = 0;
            var result = new UsersTableRowModel[S.Db.Users.Count()];
            foreach (var user in S.Db.Users)
            {
                var pubCnt = user.Commentaries.Count() + user.Posts.Count();
                result [i++] = new UsersTableRowModel()
                {
                    Name = user.UserName,
                    Role = await S.UserManager.GetRolesAsync(user).ThenDo(r => r.FirstElement()),
                    ActCnt = user.Actions.Count(),
                    PubCnt = user.Commentaries.Count() + user.Posts.Count(),
                    RepPPub = user.Reports.Where(r => r.PostObject != null || r.CommentaryObject != null).Count() / (double)pubCnt
                };
            }
            
            return Json(result);
        }
    }
}