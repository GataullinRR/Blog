using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class PermissionsService : ServiceBase
    {
        public const int MAX_POST_EDITS_FOR_STANDARD_USER = 3;
        public const int MAX_COMMENTARY_EDITS_FOR_STANDARD_USER = 1;

        public PermissionsService(ServicesProvider services) : base(services)
        {
        }

        public async Task ValidateEditPostAsync(Post post)
        {
            if (!await CanEditPostAsync(post))
            {
                throw new UnauthorizedAccessException($"The post \"{post.Title}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditPostAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                return (user.UserName == post.Author.UserName
                            && user.Status.State == ProfileState.ACTIVE
                            && post.CreationTime - DateTime.Now < TimeSpan.FromDays(3)
                            && post.Edits.Where(e => e.EditAuthor == post.Author).Count() < MAX_POST_EDITS_FOR_STANDARD_USER)
                       || await Services.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR));
            }
        }

        public async Task<bool> CanEditPostTitleAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                return await CanEditPostAsync(post)
                       && await Services.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR));
            }
        }

        public async Task ValidateCreatePostAsync()
        {
            if (!await CanCreatePostAsync())
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanCreatePostAsync()
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                return await Services.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.USER))
                    && user.Status.State == ProfileState.ACTIVE;
            }
        }

        public async Task ValidateEditCommentaryAsync(Commentary comment)
        {
            if (!await CanEditCommentaryAsync(comment))
            {
                throw new UnauthorizedAccessException($"The post \"{comment.Id}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditCommentaryAsync(Commentary comment)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null)
            {
                return false;
            }
            else
            {
                return (!comment.IsHidden
                        && user.UserName == comment.Author.UserName
                        && user.Status.State == ProfileState.ACTIVE
                        && comment.CreationTime - DateTime.Now < TimeSpan.FromDays(1)
                        && comment.Edits.Count(e => e.EditAuthor == user) < 1)
                     || await Services.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR));  
            }
        }

        public async Task ValidateResetPasswordAsync(User targetUser)
        {
            if (!await CanRestorePasswordAsync(targetUser))
            {
                throw new UnauthorizedAccessException($"Can not restore password for user \"{targetUser.UserName}\"");
            }
        }
        public async Task<bool> CanRestorePasswordAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return ((targetUser.Status.LastPasswordRestoreAttempt ?? DateTime.UtcNow.AddDays(-999)) - DateTime.UtcNow).TotalMinutes.Abs() > 30
                            && targetUser.EmailConfirmed
                            && targetUser.Status.State == ProfileState.ACTIVE
                            && await Services.UserManager.IsInOneOfTheRolesAsync(targetUser, Roles.NOT_RESTRICTED)
                        && currentUser.Id == targetUser.Id;
            }
        }

        public async Task ValidateChangePasswordAsync(User targetUser)
        {
            if (!await CanChangePasswordAsync(targetUser))
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanChangePasswordAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return targetUser.Id == currentUser.Id
                    && targetUser.EmailConfirmed
                    && targetUser.Status.State == ProfileState.ACTIVE
                    && await Services.UserManager.IsInOneOfTheRolesAsync(targetUser, Roles.NOT_RESTRICTED);
            }
        }

        public async Task ValidateBanUserAsync(User targetUser)
        {
            if (!await CanBanUserAsync(targetUser))
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanBanUserAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return targetUser.Status.State.IsOneOf(ProfileState.ACTIVE, ProfileState.RESTRICTED)
                    && await Services.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && (await Services.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await Services.UserManager.GetRolesAsync(currentUser)).Single());
            }
        }

        public async Task ValidateUnbanUserAsync(User targetUser)
        {
            if (!await CanUnbanUserAsync(targetUser))
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanUnbanUserAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return targetUser.Status.State.IsOneOf(ProfileState.BANNED)
                    && await Services.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && (await Services.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await Services.UserManager.GetRolesAsync(currentUser)).Single());
            }
        }

        public async Task<bool> CanSeePrivateInformationAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return currentUser.Id == targetUser.Id
                    || await Services.UserManager.IsInOneOfTheRolesAsync(targetUser, Roles.MODERATOR, Roles.OWNER)
                    || await Services.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.NOT_RESTRICTED);
            }
        }

        public async Task<bool> CanEditProfileAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return await CanSeePrivateInformationAsync(targetUser)
                    && ((await Services.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await Services.UserManager.GetRolesAsync(currentUser)).Single())
                        || currentUser.Id == targetUser.Id);
            }
        }

        public async Task ValidateReportAsync(IReportObject reportObject)
        {
            if (!await CanReportAsync(reportObject))
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanReportAsync(IReportObject reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return !reportObject.Reports.Any(r => r.Reporter.Id == currentUser.Id);
            }
        }

        public async Task ValidateAccessModeratorsPanelAsync()
        {
            if (!await CanAccessModeratorsPanelAsync())
            {
                throw new UnauthorizedAccessException();
            }
        }
        public async Task<bool> CanAccessModeratorsPanelAsync()
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null)
            {
                return false;
            }
            else
            {
                return await Services.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.MODERATOR);
            }
        }

        async Task<User> getCurrentUserOrNullAsync()
        {
            return await Services.UserManager.GetUserAsync(Services.HttpContext.User);
        }
    }
}
