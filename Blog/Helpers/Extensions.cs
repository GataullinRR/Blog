using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog
{
    public static class Extensions
    {
        public static string AbsoluteContent(this IUrlHelper url, string contentPath)
        {
            HttpRequest request = url.ActionContext.HttpContext.Request;
            return new Uri(new Uri(request.Scheme + "://" + request.Host.Value), url.Content(contentPath)).ToString();
        }

        public static bool IsInOneOfTheRoles(this ClaimsPrincipal user, params string[] roles)
        {
            return roles
                .Select(rs => rs.Split(","))
                .Flatten()
                .Any(r => user.IsInRole(r));
        }

        public static async Task<bool> IsInOneOfTheRolesAsync(this UserManager<User> userManager, User user, params string[] roles)
        {
            foreach (var role in roles.Select(rs => rs.Split(",")).Flatten())
            {
                var isIn = await userManager.IsInRoleAsync(user, role);
                if (isIn)
                {
                    return true;
                }
            }

            return false;
        }

        public static IIncludableQueryable<Commentary, User> IncludeAuthor(this DbSet<Commentary> commentaries)
        {
            return commentaries.Include(c => c.Author)
                    .ThenInclude(a => a.Profile)
                    .Include(c => c.Author)
                    .ThenInclude(a => a.Status)
                    .Include(c => c.Author);
        }
        public static IIncludableQueryable<Post, User> IncludeAuthor(this DbSet<Post> posts)
        {
            return posts.Include(c => c.Author)
                    .ThenInclude(a => a.Profile)
                    .Include(c => c.Author)
                    .ThenInclude(a => a.Status)
                    .Include(c => c.Author);
        }
        public static IIncludableQueryable<User, ProfileStatus> IncludeAll(this DbSet<User> users)
        {
            return users.Include(a => a.Profile)
                    .Include(a => a.Status);
        }

        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static Lazy<T> GetLazyService<T>(this IServiceProvider serviceProvider)
        {
            return new Lazy<T>(() => (T)serviceProvider.GetService(typeof(T)));
        }

        public static void UpdateStatistic(this ViewStatistic viewStatistic, User currentUser)
        {
            if (currentUser != null && currentUser.Status.State == ProfileState.ACTIVE)
            {
                viewStatistic.RegistredUserViews++;
            }
            viewStatistic.TotalViews++;
        }
    }
}
