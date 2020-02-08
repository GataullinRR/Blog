using Blog.Services;
using DBModels;
using Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Utilities.Types;
using System.Threading;
using Utilities;
using System.Reflection;

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

        readonly static Dictionary<Key, CacheHandlerDelegate> _cacheHandlers = new Dictionary<Key, CacheHandlerDelegate>();

        static CustomResponseCachingMiddleware()
        {
            _cacheHandlers = (from t in Assembly.GetExecutingAssembly().DefinedTypes
                              from mi in t.GetMethods(BindingFlags.Static | BindingFlags.Public)
                              let info = mi.GetCustomAttribute<CustomResponseCacheHandlerAttribute>()
                              where info != null
                              let @delegate = mi.CreateDelegate(typeof(CacheHandlerDelegate)) as CacheHandlerDelegate
                              where @delegate != null
                              select new { info.CacheEntryKey, d = @delegate }).ToDictionary(v => v.CacheEntryKey, v => v.d);
        }

        class CacheManagingFeature : ICacheManagerFeature
        {
            public static ICacheManagerFeature Instance { get; } = new CacheManagingFeature(CacheStorage.Instance);

            readonly CacheStorage _cacheStorage;
            readonly ConcurrentDictionary<Key, object> _requestData = new ConcurrentDictionary<Key, object>();

            CacheManagingFeature(CacheStorage cacheStorage)
            {
                _cacheStorage = cacheStorage ?? throw new ArgumentNullException(nameof(cacheStorage));
                _cacheStorage.Expired += _cacheStorage_Expired;
            }

            void _cacheStorage_Expired(Key key)
            {
                if (key != null)
                {
                    _requestData.Remove(key, out _);
                }
            }

            public async Task ResetCacheAsync(Key cacheKey)
            {
                if (cacheKey == null)
                {
                    throw new ArgumentNullException();
                }

                await _cacheStorage.TryResetCacheAsync(cacheKey);
            }

            public async Task SetRequestDataAsync<T>(Key cacheKey, T data) where T : class
            {
                if (cacheKey == null)
                {
                    throw new ArgumentNullException();
                }

                _requestData.TryAdd(cacheKey, data);
            }

            public async Task<T> GetRequestDataAsync<T>(Key cacheKey) where T : class
            {
                if (cacheKey == null)
                {
                    throw new ArgumentNullException();
                }

                return (T)_requestData[cacheKey];
            }
        }

        class CacheStorage
        {
            public static CacheStorage Instance { get; } = new CacheStorage();

            readonly Dictionary<string, CacheEntry> _cache = new Dictionary<string, CacheEntry>();
            readonly SemaphoreSlim _locker = new SemaphoreSlim(1);

            public event Action<Key> Expired;

            CacheStorage()
            {
                expirationLoopAsync();
            }

            public async Task TryAddCacheAsync(string key, CacheEntry entry)
            {
                using (await _locker.AcquireAsync())
                {
                    if (_cache.Keys.NotContains(key))
                    {
                        _cache.Add(key, entry);
                    }
                }
            }

            public async Task<CacheEntry> TryGetCacheAsync(string key)
            {
                using (await _locker.AcquireAsync())
                {
                    var has = _cache.TryGetValue(key, out var cache);
                    return has ? cache : null;
                }
            }

            public async Task<CacheEntry> TryGetCacheByKeyAsync(Key key)
            {
                using (await _locker.AcquireAsync())
                {
                    var value = _cache.SingleOrDefault(kvp => kvp.Value.CachingInfo.CacheEntryKey == key);
                    return value.Equals(default(KeyValuePair<string, CacheEntry>))
                        ? null
                        : value.Value;
                }
            }

            public async Task TryResetCacheAsync(Key key)
            {
                using (await _locker.AcquireAsync())
                {
                    _cache.RemoveAll((k, e) => e.CachingInfo.CacheEntryKey == key);
                    Expired?.Invoke(key);
                }
            }

            async void expirationLoopAsync()
            {
                await ThreadingUtils.ContinueAtDedicatedThread();

                while (true)
                {
                    await Task.Delay(5000);

                    try
                    {
                        using (await _locker.AcquireAsync())
                        {
                            var time = DateTime.UtcNow;
                            var removed = _cache.RemoveAll((key, e) => e.ServerCacheExpirationTime < time);
                            foreach (var key in removed)
                            {
                                Expired?.Invoke(key);
                            }
                        }
                    }
                    catch
                    {
                        DebugUtils.Break();
                    }
                }
            }
        }

        class CacheEntry
        {
            public CustomResponseCacheAttribute CachingInfo { get; }
            public byte[] ResponseBody { get; }
            public int StatusCode { get; }
            public IHeaderDictionary Headers { get; }
            public string ScopeUserName { get; }
            public DateTime ServerCacheExpirationTime { get; }

            public CacheEntry(CustomResponseCacheAttribute cachingInfo, byte[] responseBody, int statusCode, IHeaderDictionary headers, string scopeUserId)
            {
                CachingInfo = cachingInfo ?? throw new ArgumentNullException(nameof(cachingInfo));
                ResponseBody = responseBody ?? throw new ArgumentNullException(nameof(responseBody));
                StatusCode = statusCode;
                Headers = headers ?? throw new ArgumentNullException(nameof(headers));
                ScopeUserName = scopeUserId;
                ServerCacheExpirationTime = DateTime.UtcNow.AddSeconds(cachingInfo.ServerCacheDuration);
            }
        }

        readonly RequestDelegate _next;

        public CustomResponseCachingMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task InvokeAsync(HttpContext httpContext, URIProviderService uriProvider, IServiceProvider serviceProvider, ILoggerFactory loggerFactory, UserManager<User> userManager)
        {
            httpContext.Features.Set<ICacheManagerFeature>(CacheManagingFeature.Instance);

            var logger = loggerFactory.CreateLogger("Response caching");
            var requetURI = uriProvider.GetCurrentRequestURI();
            var cacheEntry = await tryGetCacheAsync();
            if (cacheEntry != null)
            {
                logger.LogInformation($"Serving response from cache...");

                switch (cacheEntry.CachingInfo.Mode)
                {
                    case CacheMode.USER_SCOPED:
                        var user = await userManager.GetUserAsync(httpContext.User);
                        if (user.UserName != cacheEntry.ScopeUserName) // User is not authorized correctly
                        {
                            await nextAndPopulateAsync();

                            return;
                        }
                        break;
                    case CacheMode.FOR_ANONYMOUS:
                        if (httpContext.User?.Identity?.Name != null)
                        {
                            await nextAndPopulateAsync();

                            return;
                        }
                        break;

                    default:
                        throw new NotSupportedException();
                }

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
                    var requestData = await CacheManagingFeature.Instance.GetRequestDataAsync<object>(cacheEntry.CachingInfo.CacheEntryKey);
                    await handler(new CacheScope(requestData, serviceProvider));
                }
            }

            async Task<CacheEntry> tryGetCacheAsync()
            {
                var key = httpContext.User?.Identity?.Name + requetURI;
                var cache = await CacheStorage.Instance.TryGetCacheAsync(key);
                if (cache == null)
                {
                    // Load public cache... (public cache has no user name before uri)
                    cache = await CacheStorage.Instance.TryGetCacheAsync(requetURI);
                }

                return cache;
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
                                    if (httpContext.User?.Identity?.Name == null)
                                    {
                                        throw new ArgumentOutOfRangeException("User must be signed in for using scoped caching!");
                                    }
                                    else
                                    {
                                        userName = httpContext.User?.Identity?.Name;
                                    }
                                    break;
                                case CacheMode.FOR_ANONYMOUS:
                                    if (httpContext.User?.Identity?.Name != null)
                                    {
                                        return;
                                    }
                                    break;

                                default:
                                    throw new NotSupportedException();
                            }

                            var cacheStorageKey = userName + requetURI;
                            var cache = new CacheEntry(cachingInfo, cacheBuffer, httpContext.Response.StatusCode, httpContext.Response.Headers, userName);
                            await CacheStorage.Instance.TryAddCacheAsync(cacheStorageKey, cache);
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
