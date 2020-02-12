using Blog.Services;
using DBModels;
using Utilities.Extensions;
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
using Utilities.Types;
using System.Reflection;
using Microsoft.AspNetCore.Routing;
using Blog.HttpContextFeatures;
using Blog.Attributes;
using Blog.Middlewares.CachingMiddleware.Policies;

namespace Blog.Middlewares
{
    /// <summary>
    /// Peeked in
    /// https://github.com/aspnet/ResponseCaching/blob/master/src/Microsoft.AspNetCore.ResponseCaching
    /// Must be set before MVC and after Authentication
    /// </summary>
    public class CustomResponseCachingMiddleware
    {
        delegate Task CacheHandlerDelegate(CacheScope cacheManager);

        readonly Dictionary<Key, CacheHandlerDelegate> _cacheHandlers = new Dictionary<Key, CacheHandlerDelegate>();
        readonly ICacheStorage _storage = new CacheStorage();
        readonly Dictionary<string, ICachePolicy> _cachePolicies = new Dictionary<string, ICachePolicy>();

        readonly RequestDelegate _next;

        public CustomResponseCachingMiddleware(RequestDelegate next, IEnumerable<ICachePolicy> cachePolicies)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));

            _cacheHandlers = (from t in Assembly.GetExecutingAssembly().DefinedTypes
                              from mi in t.GetMethods(BindingFlags.Static | BindingFlags.Public)
                              let info = mi.GetCustomAttribute<CustomResponseCacheHandlerAttribute>()
                              where info != null
                              let @delegate = mi.CreateDelegate(typeof(CacheHandlerDelegate)) as CacheHandlerDelegate
                              where @delegate != null
                              select new { info.CacheEntryKey, d = @delegate }).ToDictionary(v => v.CacheEntryKey, v => v.d);
            new CacheCleaner(512 * 1024 * 1024, _storage);

            foreach (var policy in cachePolicies)
            {
                var name = policy.GetType().Name;
                _cachePolicies.Add(name, policy);
            }
        }

        public async Task InvokeAsync(HttpContext httpContext, URIProviderService uriProvider, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, UserManager<User> userManager)
        {
            var logger = loggerFactory.CreateLogger("Response caching");

            var requestURI = uriProvider.GetCurrentRequestURI();
            var cacheKey = new CacheIdentity(requestURI);
            var cacheEntry = await tryGetCacheAsync();

            var manager = new CacheManagerFeature(_storage, cacheEntry?.IsRequestDataSet ?? false, cacheEntry?.RequestData);
            httpContext.Features.Set<ICacheManagerFeature>(manager);

            if (cacheEntry != null)
            {
                logger.LogInformation($"Serving response from cache...");

                await setResponseAsync();
                await executeHandlerAsync();
            }
            else
            {
                await nextAndPopulateAsync();
            }

            ///////////////////////////////////////////////////////////////////////

            async Task setResponseAsync()
            {
                httpContext.Response.StatusCode = cacheEntry.StatusCode;
                foreach (var header in cacheEntry.Headers)
                {
                    httpContext.Response.Headers[header.Key] = header.Value;
                }
                await httpContext.Response.Body.WriteAsync(cacheEntry.ResponseBody);
            }

            async Task executeHandlerAsync()
            {
                var has = _cacheHandlers.TryGetValue(cacheEntry.CachingInfo.CacheEntryKey, out var handler);
                if (has)
                {
                    var scope = new CacheScope(cacheEntry.IsRequestDataSet, cacheEntry.RequestData, serviceProvider);
                    await handler(scope);
                }
            }

            async Task<CacheEntry> tryGetCacheAsync()
            {
                var cache = await _storage.TryGetAsync(cacheKey);
                if (cache != null)
                {
                    foreach (var c in cache)
                    {
                        if (await c.Policy.CanBeServedAsync(httpContext, serviceProvider, c.PolicyMetadata))
                        {
                            return c;
                        }
                    }
                }

                return null;
            }

            async Task nextAndPopulateAsync()
            {
                byte[] cacheBuffer = null;
                {
                    var originalBodyStream = httpContext.Response.Body;
                    var cacheStream = new MemoryStream();
                    httpContext.Response.Body = cacheStream;

                    try
                    {
                        // Allow MVC action to execute (so that we will be able to get endpoint info later)
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
                    var cachingInfo = tryGetCacheAttribute();
                    if (cachingInfo != null)
                    {
                        if (cachingInfo.CacheDuration > 0)
                        {
                            var policy = _cachePolicies[cachingInfo.CachePolicy];
                            if (await policy.CanBeSavedAsync(httpContext, serviceProvider))
                            {
                                var routeData = httpContext.Features
                                    .Get<RouteDataProviderFeature>().RouteData
                                    .ToArray();
                                var cache = new CacheEntry(cachingInfo,
                                                            cacheBuffer,
                                                            httpContext.Response.StatusCode,
                                                            httpContext.Response.Headers,
                                                            manager.IsRequestDataSet,
                                                            manager.RequestData,
                                                            routeData,
                                                            policy,
                                                            await policy.GenerateMetadata(httpContext, serviceProvider));
                                await _storage.TryAddAsync(cacheKey, cache);
                            
                                logger.LogInformation($"Response for request to {requestURI} has been added to cache for {cachingInfo.CacheDuration}s");
                            }
                        }
                    }

                    CustomResponseCacheAttribute tryGetCacheAttribute()
                    {
                        // Does not work for page handlers
                        var attr = endpoint.Metadata.GetMetadata<CustomResponseCacheAttribute>();
                        if (attr == null) // Fallback for page handlers
                        {
                            var attributesProvider = httpContext.Features
                                .Select(f => f.Value as PageHandlerAttributesProviderFeature)
                                .SkipNulls()
                                .FirstOrDefault();
                            attr = attributesProvider == null
                                ? attr
                                : attributesProvider.Attributes.Select(a => a as CustomResponseCacheAttribute)
                                    .SkipNulls()
                                    .FirstOrDefault();
                        }

                        return attr;
                    }
                }
            }
        }
    }
}
