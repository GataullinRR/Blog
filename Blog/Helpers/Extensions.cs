using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Utilities.Extensions;
using Utilities.Types;

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

        public static T GetService<T>(this IServiceProvider serviceProvider)
        {
            return (T)serviceProvider.GetService(typeof(T));
        }

        public static Lazy<T> GetLazyService<T>(this IServiceProvider serviceProvider)
        {
            return new Lazy<T>(() => (T)serviceProvider.GetService(typeof(T)));
        }

        /// <summary>
        /// Does not work as @Json.Serialize for objects
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string ToJSON(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static async Task<IQueryable<User>> GetUsersInRoleAsync(this BlogContext db, string role)
        {
            var roleId = (await db.Roles.FirstAsync(r => r.Name == role)).Id;
            var usersIds = db.UserRoles.Where(r => r.RoleId == roleId).Select(ur => ur.UserId);

            return db.Users.Where(u => usersIds.Contains(u.Id));
        }

        public static string ToCurrentTimeRelativeString(this TimeSpan span)
        {
            if (span < TimeSpan.FromSeconds(60))
            {
                return "just now";
            }
            else if (span < TimeSpan.FromMinutes(60))
            {
                return $"{span.TotalMinutes.ToInt32()} minutes ago";
            }
            else if (span < TimeSpan.FromHours(24))
            {
                return $"{span.TotalHours.Round()} hours ago";
            }
            else
            {
                return $"{span.TotalDays.Round()} days ago";
            }
        }

        public static string ToHoursDateString(this DateTime dateTime)
        {
            return dateTime.ToString("hh:mm dd.MM.yyyy");
        }

        public static IDisposable SaveChangesMode(this BlogContext db)
        {
            return new DisposingAction(() => db.SaveChanges());
        }

        public static string GetUTF8StringOrDefault(this ISession session, string key, string defaultValue = null)
        {
            var has = session.TryGetValue(key, out byte[] ddd);
         
            return has
                ? Encoding.UTF8.GetString(ddd)
                : defaultValue;
        }

        public static bool IsSuccessStatusCode(this HttpResponse response)
        {
            return response.StatusCode >= 200 && response.StatusCode <= 299;
        }
    }
}
