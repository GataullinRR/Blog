using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    class UserScopedCachePolicy : ICachePolicy
    {
        readonly string _userName;

        public UserScopedCachePolicy(string userName)
        {
            _userName = userName;
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return httpContext.User?.Identity?.Name == _userName;
        }
    }
}
