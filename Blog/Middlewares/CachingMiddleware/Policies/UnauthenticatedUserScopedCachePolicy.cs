﻿using Blog.Attributes;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    [Service(ServiceType.SINGLETON, typeof(ICachePolicy))]
    class UnauthenticatedUserScopedCachePolicy : ICachePolicy
    {
        public async Task<bool> CanBeSavedAsync(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return httpContext.User?.Identity?.Name == null;
        }

        public async Task<object> GenerateMetadata(HttpContext httpContext, IServiceProvider serviceProvider)
        {
            return null;
        }

        public async Task<bool> CanBeServedAsync(HttpContext httpContext, IServiceProvider serviceProvider, object metadata)
        {
            return httpContext.User?.Identity?.Name == null;
        }
    }
}
