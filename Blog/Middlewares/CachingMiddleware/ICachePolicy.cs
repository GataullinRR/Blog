using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    interface ICachePolicy
    {
        Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider);
    }
}
