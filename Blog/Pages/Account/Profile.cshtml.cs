using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Utilities.Extensions;
using Blog.Attributes;
using Blog.Middlewares.CachingMiddleware.Policies;

namespace Blog.Pages.Account
{
    public class ProfileModel : PageModelBase
    {
        public Models.ProfileModel Profile { get; private set; }

        public ProfileModel(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [CustomResponseCache(3 * 60, CachePolicy.UNATHORZED_USER_SCOPED, CacheManagerService.PROFILE_GET_CACHE_KEY)]
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                var currentUser = await S.UserManager.GetUserAsync(HttpContext.User);
                if (currentUser == null)
                {
                    throw new UnauthorizedAccessException("Can't determine target user");
                }
                else
                {
                    id = currentUser.Id;
                }
            }

            var targetUser = await S.Db.Users.AsNoTracking()
                .Include(u => u.Actions)
                .ThenInclude(p => p.ProfileObject)
                .Include(u => u.Actions)
                .ThenInclude(p => p.PostObject)
                .Include(u => u.Actions)
                .ThenInclude(p => p.CommentaryObject)

                .Include(u => u.Violations)
                .ThenInclude(v => v.Reporter)
                .Include(u => u.Violations)
                .ThenInclude(p => p.ProfileObject)
                .Include(u => u.Violations)
                .ThenInclude(p => p.PostObject)
                .Include(u => u.Violations)
                .ThenInclude(p => p.CommentaryObject)

                .Include(u => u.Reports)
                .ThenInclude(v => v.Reporter)
                .Include(u => u.Reports)
                .ThenInclude(p => p.ProfileObject)
                .Include(u => u.Reports)
                .ThenInclude(p => p.PostObject)
                .Include(u => u.Reports)
                .ThenInclude(p => p.CommentaryObject)

                .Include(u => u.ReportedReports)
                .ThenInclude(v => v.Reporter)
                .Include(u => u.ReportedReports)
                .ThenInclude(p => p.ProfileObject)
                .Include(u => u.ReportedReports)
                .ThenInclude(p => p.PostObject)
                .Include(u => u.ReportedReports)
                .ThenInclude(p => p.CommentaryObject)

                .Include(u => u.Profile)
                .ThenInclude(p => p.ViewStatistic)
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == id);
            Profile = new Models.ProfileModel()
            {
                Permissions = await S.Permissions.GetProfilePermissionsAsync(id),
                ContactHelpEmail = await S.ContactEmailProvider.GetHelpContactEmailAsync(),
                IsCurrentUser = targetUser.UserName == S.HttpContext.User.Identity.Name,
                TargetUser = new UserProfileModel()
                {
                    About = targetUser.Profile.About,
                    IsEmailConfirmed = targetUser.EmailConfirmed,
                    Role = targetUser.Role.GetEnumValueDescription(),
                    UserId = targetUser.Id,
                    User = targetUser.UserName,
                    ViewStatistic = targetUser.Profile.ViewStatistic,
                    EMail = targetUser.Email,
                    RegistrationDate = targetUser.Profile.RegistrationDate,
                    State = targetUser.Status.State
                },
                Actions = targetUser.Actions.Select(a => new UserActionModel()
                {
                    ActionDate = a.ActionDate,
                    ActionObject = a.ActionObject,
                    Type = a.ActionType,
                    IsSelfAction = a.ProfileObject?.Id == targetUser.Profile.Id,
                }).ToArray(),
                Posts = await S.Db.Posts.Where(p => p.Author.Id == targetUser.Id)
                    .Select(p => new PostProfileModel()
                    {
                        BodyPreview = p.BodyPreview,
                        CreationTime = p.CreationTime,
                        IsDeleted = p.IsDeleted,
                        ModerationState = p.ModerationInfo.State,
                        PostId = p.Id,
                        Title = p.Title,
                    }).ToListAsync(),
                ProfileImage = new ProfileImageModel()
                {
                    RelativeUri = targetUser.Profile.Image
                },
                ReportedReports = targetUser.ReportedReports.Select(r => new ReportModel()
                {
                    Reporter = r.Reporter.UserName,
                    ReporterId = r.Reporter.Id,
                    CreationTime = r.CreationDate,
                    Object = r.Object
                }).ToArray(),
                Reports = targetUser.Reports.Select(r => new ReportModel()
                {
                    Reporter = r.Reporter.UserName,
                    ReporterId = r.Reporter.Id,
                    CreationTime = r.CreationDate,
                    Object = r.Object
                }).ToArray(),
                Violations = targetUser.Violations.Select(v => new ViolationModel()
                {
                    CreationTime = v.CreationDate,
                    Object = v.Object,
                    Description = v.Description,
                    Reporter = v.Reporter.UserName,
                    ReporterId = v.Reporter.Id
                }).ToArray()
            };

            await S.DbUpdator.UpdateViewStatisticAsync(S.HttpContext.User?.Identity?.IsAuthenticated ?? false, Profile.TargetUser.ViewStatistic);
            await S.Db.SaveChangesAsync();

            return Page();
        }
    }
}