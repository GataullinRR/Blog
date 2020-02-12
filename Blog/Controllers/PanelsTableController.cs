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

        [CustomResponseCache(3600 * 24, CachePolicy.AUTHORIZED_USER_SCOPED), AJAX]
        public async Task<IActionResult> LoadFullUsersTableAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            return await generateTableData(S.Db.Users);
        }

        [CustomResponseCache(3600 * 24, CachePolicy.AUTHORIZED_USER_SCOPED), AJAX]
        public async Task<IActionResult> LoadModeratorsUsersTableAsync([Required]string id)
        {
            if (ModelState.IsValid)
            {
                var targetModerator = await S.UserManager.FindByIdAsync(id);
                await S.Permissions.ValidateAccessModeratorsPanelAsync(targetModerator);

                return await generateTableData(targetModerator.ModeratorsGroup.TargetUsers.AsQueryable());
            }
            else
            {
                throw new Exception();
            }
        }

        async Task<JsonResult> generateTableData(IQueryable<User> users)
        {
            S.Db.ChangeTracker.LazyLoadingEnabled = false;
            var query = users
                .Select(user => new
                {
                    Name = $"{user.Id}|{user.UserName}",
                    RoleId = S.Db.UserRoles.First(r => r.UserId == user.Id).RoleId,
                    ActCnt = user.Actions.Count(),
                    State = user.Status.State,
                    RegTime = user.Profile.RegistrationDate,
                    PubCnt = user.Commentaries.Count() + user.Posts.Count(),
                    RepCnt = user.Reports.Where(r => r.PostObject != null || r.CommentaryObject != null).Count()
                })
                .ToArray();
            var roles = S.Db.Roles.ToDictionary(r => r.Id, r => r.Name);
            var result = query
                .Select(user => new UsersTableRowModel()
                {
                    Name = user.Name,
                    Role = roles[user.RoleId],
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