using DBModels;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class PermissionsService
    {
        public const int MAX_POST_EDITS_FOR_STANDARD_USER = 3;
        public const int MAX_COMMENTARY_EDITS_FOR_STANDARD_USER = 1;

        public bool CanEditPost(ClaimsPrincipal user, Post post)
        {
            return (user.Identity.Name == post.Author.UserName && 
                    post.Date - DateTime.Now < TimeSpan.FromDays(3) && 
                    post.Edits.Where(e => e.Author == post.Author).Count() < MAX_POST_EDITS_FOR_STANDARD_USER)
                    || user.IsInOneOfTheRoles(Roles.ADMIN, Roles.MODERATOR);
        }
        public void ValidateEditPost(ClaimsPrincipal user, Post post)
        {
            if (!CanEditPost(user, post))
            {
                throw new UnauthorizedAccessException($"The post \"{post.Title}\" can not be edited by the user \"{user.Identity.Name}\"");
            }
        }

        public void ValidateEditCommentary(ClaimsPrincipal user, Commentary comment)
        {
            if (!CanEditCommentary(user, comment))
            {
                throw new UnauthorizedAccessException($"The post \"{comment.Id}\" can not be edited by the user \"{user.Identity.Name}\"");
            }
        }

        public bool CanEditCommentary(ClaimsPrincipal user, Commentary comment)
        {
            return (user.Identity.Name == comment.Author.UserName &&
                    comment.Date - DateTime.Now < TimeSpan.FromDays(1))
                    || user.IsInOneOfTheRoles(Roles.ADMIN, Roles.MODERATOR);
        }
    }
}
