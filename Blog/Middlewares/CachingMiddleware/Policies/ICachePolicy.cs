using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Blog.Attributes;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    public interface ICachePolicy
    {
        /// <summary>
        /// Executed after MVC if handler/action had <see cref="CustomResponseCachingAttribute"/> 
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider);
        /// <summary>
        /// Executed if cache can be saved
        /// </summary>
        /// <param name="httpContext"></param>
        /// <param name="serviceProvider"></param>
        /// <returns></returns>
        Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider);
        Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata);
    }
}
