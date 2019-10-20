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
    public class PermissionsService
    {
        public const int MAX_POST_EDITS_FOR_STANDARD_USER = 3;
        public const int MAX_COMMENTARY_EDITS_FOR_STANDARD_USER = 1;

        readonly UserManager<User> _userManager;

        public PermissionsService(UserManager<User> userManager)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public async Task ValidateEditPostAsync(ClaimsPrincipal user, Post post)
        {
            if (!await CanEditPostAsync(user, post))
            {
                throw new UnauthorizedAccessException($"The post \"{post.Title}\" can not be edited by the user \"{user.Identity.Name}\"");
            }
        }
        public async Task<bool> CanEditPostAsync(ClaimsPrincipal user, Post post)
        {
            return (user.Identity.Name == post.Author.UserName 
                    && (await _userManager.GetUserAsync(user)).Status.State == ProfileState.ACTIVE
                    && post.CreationTime - DateTime.Now < TimeSpan.FromDays(3)
                    &&  post.Edits.Where(e => e.EditAuthor == post.Author).Count() < MAX_POST_EDITS_FOR_STANDARD_USER)
                    || user.IsInOneOfTheRoles(Roles.ADMIN, Roles.MODERATOR);
        }

        public async Task ValidateEditCommentaryAsync(ClaimsPrincipal user, Commentary comment)
        {
            if (!await CanEditCommentaryAsync(user, comment))
            {
                throw new UnauthorizedAccessException($"The post \"{comment.Id}\" can not be edited by the user \"{user.Identity.Name}\"");
            }
        }
        public async Task<bool> CanEditCommentaryAsync(ClaimsPrincipal user, Commentary comment)
        {
            var thisUser = await _userManager.GetUserAsync(user);
            if (thisUser == null)
            {
                return false;
            }
            else
            {
                return (user.Identity.Name == comment.Author.UserName
                    && thisUser.Status.State == ProfileState.ACTIVE
                    && comment.CreationTime - DateTime.Now < TimeSpan.FromDays(1)
                    && comment.Edits.Count(e => e.EditAuthor == thisUser) < 1)
                    || user.IsInOneOfTheRoles(Roles.ADMIN, Roles.MODERATOR);  
            }
        }

        public async Task ValidateResetPasswordAsync(ClaimsPrincipal currentUser, User user)
        {
            if (!await CanRestorePasswordAsync(currentUser, user))
            {
                throw new UnauthorizedAccessException($"Can not restore password for user \"{user.UserName}\"");
            }
        }
        public async Task<bool> CanRestorePasswordAsync(ClaimsPrincipal currentUser, User user)
        {
            return ((user.Status.LastPasswordRestoreAttempt ?? DateTime.UtcNow) - DateTime.UtcNow).TotalMinutes.Abs() > 30
                && user.EmailConfirmed 
                && user.Status.State == ProfileState.ACTIVE
                && await _userManager.IsInOneOfTheRolesAsync(user, Roles.NOT_RESTRICTED)
                && (!currentUser?.Identity?.IsAuthenticated ?? true);
        }


        public void ValidateChangePassword(ClaimsPrincipal user, User userModel)
        {
            if (!CanChangePassword(user, userModel))
            {
                throw new UnauthorizedAccessException();
            }
        }
        public bool CanChangePassword(ClaimsPrincipal user, User userModel)
        {
            return user.Identity.IsAuthenticated 
                && user.Identity.Name == userModel.UserName
                && userModel.EmailConfirmed
                && userModel.Status.State == ProfileState.ACTIVE
                && user.IsInOneOfTheRoles(Roles.NOT_RESTRICTED);
        }
    }
}
