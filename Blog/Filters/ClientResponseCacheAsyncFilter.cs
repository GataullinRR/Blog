using Microsoft.AspNetCore.Mvc.Filters;
using Blog.Attributes;
using System.Threading.Tasks;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;

namespace Blog.Filters
{
    public class ClientResponseCacheAsyncFilter : IAsyncPageFilter, IAsyncActionFilter
    {
        public ClientResponseCacheAsyncFilter()
        {

        }

        public Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }

        public async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context,
                                                      PageHandlerExecutionDelegate next)
        {
            var cacheAttribute = context.HandlerMethod.MethodInfo.GetCustomAttribute<ClientResponseCacheAttribute>();
            trySetHeaders(cacheAttribute, context.HttpContext);

            await next.Invoke();
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cacheAttribute = (ClientResponseCacheAttribute)context.ActionDescriptor.EndpointMetadata.FirstOrDefault(m => m.GetType() == typeof(ClientResponseCacheAttribute));
            trySetHeaders(cacheAttribute, context.HttpContext);

            await next();
        }

        void trySetHeaders(ClientResponseCacheAttribute cacheInfo, HttpContext context)
        {
            if (cacheInfo != null)
            {
                var headers = context.Response.GetTypedHeaders();
                headers.CacheControl = new CacheControlHeaderValue
                {
                    Public = true,
                    MaxAge = TimeSpan.FromSeconds(cacheInfo.CacheDuration)
                };
            }
        }
    }
}
