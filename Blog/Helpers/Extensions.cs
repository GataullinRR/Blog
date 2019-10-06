using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    }
}
