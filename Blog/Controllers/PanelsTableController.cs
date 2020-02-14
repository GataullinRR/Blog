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
using DBModels;
using System.ComponentModel.DataAnnotations;
using Blog.Attributes;
using Blog.Middlewares.CachingMiddleware.Policies;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class PanelsTableController : ControllerBase
    {
        public static readonly string MODERATORS_PANEL_USERS_URI = getURIToAction(nameof(PanelsTableController), nameof(LoadModeratorsUsersTableAsync));
        public static readonly string ALL_USERS_URI = getURIToAction(nameof(PanelsTableController), nameof(LoadFullUsersTableAsync));

        public PanelsTableController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        class UsersTableRowModel
        {
            public string Name { get; set; }
            public string Role { get; set; }
            public long RegTime { get; set; }
            public string State { get; set; }
            public int ActCnt { get; set; }
            public int PubCnt { get; set; }
            public double RepPPub { get; set; }
        }

        [ServerResponseCache(3600 * 24, CachePolicy.ADMINISTRATOR_PANEL_SCOPED), AJAX]
        public async Task<IActionResult> LoadFullUsersTableAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            return await generateTableData(S.Db.Users.AsNoTracking());
        }

        [ServerResponseCache(3600 * 24, CachePolicy.MODERATOR_PANEL_SCOPED), AJAX]
        public async Task<IActionResult> LoadModeratorsUsersTableAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var targetModeratorGroup = await S.Db.ModeratorsGroups
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == id);
                await S.Permissions.ValidateAccessModeratorsPanelUsersTableAsync(targetModeratorGroup);

                var users = S.Db.Users
                    .Where(u => u.ModeratorsInChargeGroup.Id == targetModeratorGroup.Id)
                    .AsNoTracking();
                return await generateTableData(users);
            }
            else
            {
                throw new Exception();
            }
        }

        async Task<JsonResult> generateTableData(IQueryable<User> users)
        {
            var query = await users.AsNoTracking()
                .Select(user => new
                {
                    Name = $"{user.Id}|{user.UserName}",
                    Role = user.Role,
                    ActCnt = user.Actions.Count(),
                    State = user.Status.State,
                    RegTime = user.Profile.RegistrationDate,
                    PubCnt = user.Commentaries.Count() + user.Posts.Count(),
                    RepCnt = user.Reports.Where(r => r.PostObject != null || r.CommentaryObject != null).Count()
                })
                .ToListAsync();
            var result = query
                .Select(user => new UsersTableRowModel()
                {
                    Name = user.Name,
                    Role = user.Role.GetEnumValueDescription(),
                    ActCnt = user.ActCnt,
                    State = user.State.GetEnumValueDescription(),
                    RegTime = new DateTimeOffset(user.RegTime).ToUnixTimeSeconds(),
                    PubCnt = user.PubCnt,
                    RepPPub = (user.RepCnt / (double)user.PubCnt).Exchange(double.NaN, 0)
                })
                .ToArray();

            return Json(result);
        }
    }
}