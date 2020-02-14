using Blog.Attributes;
using Blog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Utilities.Extensions;
using System.Threading.Tasks;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    [Service(ServiceType.SINGLETON, typeof(ICachePolicy))]
    class ModeratorPanelScopedCachePolicy : ICachePolicy
    {
        public async Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return await tryGetModeratorsGroupIdAsync(httpContext, serviceProvider) != null;
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata)
        {
            return await tryGetModeratorsGroupIdAsync(httpContext, serviceProvider) == metadata.To<int>();
        }

        public async Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return (await tryGetModeratorsGroupIdAsync(httpContext, serviceProvider)).Value;
        }

        async Task<int?> tryGetModeratorsGroupIdAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var utilities = serviceProvider.GetService<UtilitiesService>();
            var userQuery = await utilities.GetCurrentUserAsQueryableAsync();
            if (userQuery == null)
            {
                return null;
            }
            else
            {
                var user = await userQuery.Include(u => u.ModeratorsGroup)
                    .Where(u => u.Role == DBModels.Role.MODERATOR)
                    .AsNoTracking()
                    .SingleOrDefaultAsync();
                return user?.ModeratorsGroup?.Id;
            }
        }
    }
}
