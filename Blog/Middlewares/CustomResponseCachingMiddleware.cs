using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    /// <summary>
    /// Peeked in
    /// https://github.com/aspnet/ResponseCaching/blob/master/src/Microsoft.AspNetCore.ResponseCaching
    /// Must be set before MVC and after Authentication
    /// </summary>
    public class CustomResponseCachingMiddleware
    {
        class CacheEntry
        {
            public CacheEntry(CustomResponseCacheAttribute cachingInfo, byte[] responseBody, int statusCode, IHeaderDictionary headers, string scopeUserId)
            {
                CachingInfo = cachingInfo ?? throw new ArgumentNullException(nameof(cachingInfo));
                ResponseBody = responseBody ?? throw new ArgumentNullException(nameof(responseBody));
                StatusCode = statusCode;
                Headers = headers ?? throw new ArgumentNullException(nameof(headers));
                ScopeUserName = scopeUserId;
            }

            public CustomResponseCacheAttribute CachingInfo { get; }
            public byte[] ResponseBody { get; }
            public int StatusCode { get; }
            public IHeaderDictionary Headers { get; }
            public string ScopeUserName { get; }
        }

        readonly RequestDelegate _next;

        public CustomResponseCachingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext, IMemoryCache cacheStorage, URIProviderService uriProvider, ILoggerFactory loggerFactory, UserManager<User> userManager)
        {
            var logger = loggerFactory.CreateLogger("Response caching");
            var requetURI = uriProvider.GetCurrentRequestURI();
            var cacheEntry = tryGetCache();
            if (cacheEntry != null)
            {
                logger.LogInformation($"Serving response from cache...");

                switch (cacheEntry.CachingInfo.Mode)
                {
                    case CacheMode.USER_SCOPED:
                        var user = await userManager.GetUserAsync(httpContext.User);
                        if (user.UserName != cacheEntry.ScopeUserName) // User is not authorized correctly
                        {
                            await nextAndPopulate();

                            return;
                        }
                        break;
                    case CacheMode.PUBLIC:

                        break;
                 
                    default:
                        throw new NotSupportedException();
                }

                httpContext.Response.StatusCode = cacheEntry.StatusCode;
                foreach (var header in cacheEntry.Headers)
                {
                    httpContext.Response.Headers[header.Key] = header.Value;
                }
                await httpContext.Response.Body.WriteAsync(cacheEntry.ResponseBody);
            }
            else
            {
                await nextAndPopulate();
            }

            CacheEntry tryGetCache()
            {
                var key = httpContext.User?.Identity?.Name + requetURI;
                var hasUserScopedCache = cacheStorage.TryGetValue(key, out CacheEntry entry);
                if (!hasUserScopedCache)
                {
                    // Load public cache...
                    cacheStorage.TryGetValue(requetURI, out entry);
                }

                return entry;
            }

            async Task nextAndPopulate()
            {
                byte[] cacheBuffer = null;
                {
                    var originalBodyStream = httpContext.Response.Body;
                    var cacheStream = new MemoryStream();
                    httpContext.Response.Body = cacheStream;

                    try
                    {
                        // Allow MVC action to execute
                        await _next(httpContext);
                    }
                    finally
                    {
                        cacheBuffer = cacheStream.ToArray();
                        await originalBodyStream.WriteAsync(cacheBuffer);
                        httpContext.Response.Body = originalBodyStream;
                    }
                }

                var endpoint = httpContext.Features.Get<IEndpointFeature>()?.Endpoint;
                // Endpoint will be null if the URL didn't match an action, e.g. a 404 response
                if (endpoint != null && httpContext.Response.IsSuccessStatusCode())
                {
                    var cachingInfo = endpoint.Metadata.GetMetadata<CustomResponseCacheAttribute>();
                    if (cachingInfo != null)
                    {
                        if (cachingInfo.ClientCacheDuration > 0)
                        {
                            if (httpContext.Response.GetTypedHeaders().CacheControl == null)
                            {
                                logger.LogWarning($"Client side cahing requested, but cache-control headers aren't set! Configure cache filters!");
                            }
                        }
                        if (cachingInfo.ServerCacheDuration > 0)
                        {
                            logger.LogInformation($"Response for request to {requetURI} has been added to cache for {cachingInfo.ServerCacheDuration}s");

                            string userName = null;
                            switch (cachingInfo.Mode)
                            {
                                case CacheMode.USER_SCOPED:
                                    var user = await userManager.GetUserAsync(httpContext.User);
                                    if (user == null)
                                    {
                                        throw new ArgumentOutOfRangeException("User must be signed in for using scoped caching!");
                                    }
                                    else
                                    {
                                        userName = user.UserName;
                                    }
                                    break;
                                case CacheMode.PUBLIC:

                                    break;

                                default:
                                    throw new NotSupportedException();
                            }

                            var cache = new CacheEntry(cachingInfo, cacheBuffer, httpContext.Response.StatusCode, httpContext.Response.Headers, userName);
                            cacheStorage.Set(userName + requetURI, cache, TimeSpan.FromSeconds(cachingInfo.ServerCacheDuration));
                        }
                    }
                }
            }
        }
    }
}
