using Blog.Attributes;
using Blog.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    [Service(ServiceType.SINGLETON, typeof(ICachePolicy))]
    class AdministratorRoleScopedCachePolicy : ICachePolicy
    {
        public async Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return await canUseAsync(httpContext, serviceProvider);
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata)
        {
            return await canUseAsync(httpContext, serviceProvider);
        }

        public async Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return null;
        }

        async Task<bool> canUseAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            var utilities = serviceProvider.GetService<UtilitiesService>();
            var user = await utilities.GetCurrentUserModelAsync();

            return user?.Role == DBModels.Role.ADMINISTRATOR;
        }
    }
}
