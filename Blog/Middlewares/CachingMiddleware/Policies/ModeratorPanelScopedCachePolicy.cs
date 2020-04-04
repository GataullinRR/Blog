using Blog.Attributes;
using Blog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Utilities.Extensions;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    [Service(ServiceType.SINGLETON, typeof(ICachePolicy))]
    class ModeratorPanelScopedCachePolicy : ICachePolicy
    {
        public async Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return await tryGetCurrentUserModeratorsGroupIdAsync(serviceProvider) != null;
        }

        public async Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return await tryGetCurrentUserModeratorsGroupIdAsync(serviceProvider);
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata)
        {
            var cacheOwner = metadata.To<int>();
            var cacheTarget = await tryGetCurrentUserModeratorsGroupIdAsync(serviceProvider);
           
            return cacheTarget == cacheOwner;
        }

        async Task<int?> tryGetCurrentUserModeratorsGroupIdAsync(IServiceProvider serviceProvider)
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
