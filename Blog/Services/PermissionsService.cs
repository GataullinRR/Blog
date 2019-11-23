using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
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
                throw buildException($"The post \"{post.Title}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditPostAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(post)
                       && ((user.UserName == post.Author.UserName
                            && post.CreationTime - DateTime.Now < TimeSpan.FromDays(3)
                            && post.Edits.Where(e => e.Author == post.Author).Count() < MAX_POST_EDITS_FOR_STANDARD_USER
                            && post.State == ModerationState.MODERATED)
                       || (await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR))
                            && user != post.Author));
            }
        }

        public async Task<bool> CanEditPostTitleAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(post)
                    && await CanEditPostAsync(post)
                    && await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR));
            }
        }

        public async Task ValidateCreatePostAsync()
        {
            if (!await CanCreatePostAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanCreatePostAsync()
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.USER));
            }
        }

        public async Task ValidateViewPostAsync(Post post, bool showLastEdit)
        {
            if (!await CanViewPostAsync(post, showLastEdit))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanViewPostAsync(Post post, bool showLastEdit)
        {
            var user = await getCurrentUserOrNullAsync();
            var result = post.State == ModerationState.MODERATED
                    && !showLastEdit
                    && !post.IsDeleted;
            if (user == null)
            {
                return result;
            }
            else
            {
                return (await S.UserManager.IsInRoleAsync(user, Roles.USER) 
                    && !post.IsDeleted)
                    || result
                    || await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR))
                    || user == post.Author && !post.IsDeleted;
            }
        }

        public async Task ValidateEditCommentaryAsync(Commentary comment)
        {
            if (!await CanEditCommentaryAsync(comment))
            {
                throw buildException($"The post \"{comment.Id}\" can not be edited by current user");
            }
        }
        public async Task<bool> CanEditCommentaryAsync(Commentary comment)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(comment)
                        && !comment.IsDeleted
                            && ((!comment.IsHidden
                                && !comment.IsDeleted
                                && user.UserName == comment.Author.UserName
                                && user.Status.State == ProfileState.ACTIVE
                                && comment.CreationTime - DateTime.Now < TimeSpan.FromDays(1)
                                && comment.Edits.Count(e => e.Author == user) < 1)
                            || await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR)));  
            }
        }

        public async Task ValidateDeletePostAsync(Post post)
        {
            if (!await CanDeletePostAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanDeletePostAsync(Post post)
        {
            return await canDeleteAsync(post);
        }
        public async Task ValidateDeleteCommentaryAsync(Commentary comment)
        {
            if (!await CanDeleteCommentaryAsync(comment))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanDeleteCommentaryAsync(Commentary comment)
        {
            return await canDeleteAsync(comment);
        }
        async Task<bool> canDeleteAsync(IDeletable deletable)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(deletable)
                    && !deletable.IsDeleted
                    && await S.UserManager.IsInOneOfTheRolesAsync(user, Roles.GetAllNotLess(Roles.MODERATOR));
            }
        }

        public async Task ValidateAddCommentaryAsync(Post post)
        {
            if (!await CanAddCommentaryAsync(post))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAddCommentaryAsync(Post post)
        {
            var user = await getCurrentUserOrNullAsync();
            if (user == null || user.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(post);
            }
        }

        public async Task ValidateResetPasswordAsync(User targetUser)
        {
            if (!await CanRestorePasswordAsync(targetUser))
            {
                throw buildException();
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
                            && await S.UserManager.IsInOneOfTheRolesAsync(targetUser, Roles.NOT_RESTRICTED)
                        && currentUser.Id == targetUser.Id;
            }
        }

        public async Task ValidateChangePasswordAsync(User targetUser)
        {
            if (!await CanChangePasswordAsync(targetUser))
            {
                throw buildException();
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
                return await isNotBlockedAsync(targetUser)
                    && targetUser.Id == currentUser.Id
                    && targetUser.EmailConfirmed
                    && targetUser.Status.State == ProfileState.ACTIVE
                    && await S.UserManager.IsInOneOfTheRolesAsync(targetUser, Roles.NOT_RESTRICTED);
            }
        }

        public async Task ValidateChangeEmailAsync(User targetUser)
        {
            if (!await CanChangeEmailAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanChangeEmailAsync(User targetUser)
        {
            return await CanChangePasswordAsync(targetUser);
        }

        public async Task ValidateBanUserAsync(User targetUser)
        {
            if (!await CanBanUserAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanBanUserAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(targetUser)
                    && targetUser.Status.State.IsOneOf(ProfileState.ACTIVE, ProfileState.RESTRICTED)
                    && await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && (await S.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await S.UserManager.GetRolesAsync(currentUser)).Single());
            }
        }

        public async Task ValidateUnbanUserAsync(User targetUser)
        {
            if (!await CanUnbanUserAsync(targetUser))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanUnbanUserAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(targetUser)
                    && targetUser.Status.State.IsOneOf(ProfileState.BANNED)
                    && await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && (await S.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await S.UserManager.GetRolesAsync(currentUser)).Single());
            }
        }

        public async Task ValidateLogoutAsync()
        {
            if (!await CanLogoutAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanLogoutAsync()
        {
            return S.HttpContext.User.Identity.IsAuthenticated;
        }

        public async Task<bool> CanSeePrivateInformationAsync(User targetUser)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return currentUser.Id == targetUser.Id
                    || await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR));
            }
        }

        public async Task<bool> CanSeeServiceInformationAsync()
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR));
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
                return await isNotBlockedAsync(targetUser)
                    && await CanSeePrivateInformationAsync(targetUser)
                    && ((await S.UserManager.GetRolesAsync(targetUser)).Single().IsLess((await S.UserManager.GetRolesAsync(currentUser)).Single())
                        || currentUser.Id == targetUser.Id);
            }
        }

        public async Task ValidateReportAsync(IReportable reportObject)
        {
            if (!await CanReportAsync(reportObject))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanReportAsync(IReportable reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(reportObject)
                    && !reportObject.Reports.Any(r => r.Reporter.Id == currentUser.Id)
                    && !(reportObject.As<Commentary>()?.IsDeleted).NullToFalse()
                    && await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR)).ThenDo(r => !r);
            }
        }

        public async Task ValidateReportViolationAsync(IReportable reportObject)
        {
            if (!await CanReportViolationAsync(reportObject))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanReportViolationAsync(IReportable reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(reportObject)
                    && await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && reportObject.Violations.All(v => v.Reporter != currentUser);
            }
        }

        public async Task ValidateMarkAsModeratedAsync(IModeratable moderatable)
        {
            if (!await CanMarkAsModeratedAsync(moderatable))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanMarkAsModeratedAsync(IModeratable reportObject)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await isNotBlockedAsync(reportObject)
                    && await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR))
                    && reportObject.State.IsOneOf(ModerationState.UNDER_MODERATION)
                    && currentUser != reportObject.Author;
            }
        }

        public async Task ValidateAccessModeratorsPanelAsync(User target)
        {
            if (!await CanAccessModeratorsPanelAsync(target))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAccessModeratorsPanelAsync(User target)
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return (await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.OWNER)) 
                        && currentUser != target)
                    || (await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.MODERATOR)
                        && currentUser == target);
            }
        }

        public async Task ValidateAccessBlogControlPanelAsync()
        {
            if (!await CanAccessBlogControlPanelAsync())
            {
                throw buildException();
            }
        }
        public async Task<bool> CanAccessBlogControlPanelAsync()
        {
            var currentUser = await getCurrentUserOrNullAsync();
            if (currentUser == null || currentUser.Status.State != ProfileState.ACTIVE)
            {
                return false;
            }
            else
            {
                return await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.OWNER);
            }
        }

        public async Task ValidateGenerateActivationLinkAsync(ActivationLinkAction activationLinkAction)
        {
            if (!await CanGenerateActivationLinkAsync(activationLinkAction))
            {
                throw buildException();
            }
        }
        public async Task<bool> CanGenerateActivationLinkAsync(ActivationLinkAction activationLinkAction)
        {
            return await CanAccessBlogControlPanelAsync();
        }

        async Task<bool> isNotBlockedAsync(object entity)
        {
            if (entity is IDeletable deletable)
            {
                return await Task.FromResult(!deletable.IsDeleted);
            }
            else
            {
                return true;
            }
        }

        async Task<User> getCurrentUserOrNullAsync()
        {
            return await S.UserManager.GetUserAsync(S.HttpContext.User);
        }

        Exception buildException(string reason = null)
        {
            S.HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;

            return new UnauthorizedAccessException(reason);
        }
    }
}