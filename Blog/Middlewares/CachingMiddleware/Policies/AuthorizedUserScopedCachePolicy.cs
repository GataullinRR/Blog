using Blog.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    [Service(ServiceType.SINGLETON, typeof(ICachePolicy))]
    class AuthorizedUserScopedCachePolicy : ICachePolicy
    {
        public async Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return httpContext.User?.Identity?.Name != null;
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata)
        {
            var userName = metadata as string;

            return httpContext.User?.Identity?.Name == userName;
        }

        public async Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return httpContext.User?.Identity?.Name;
        }
    }
}
