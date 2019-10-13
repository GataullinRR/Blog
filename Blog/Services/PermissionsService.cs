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
    }
}
