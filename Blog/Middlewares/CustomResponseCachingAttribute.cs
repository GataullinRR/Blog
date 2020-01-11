using Microsoft.AspNetCore.Mvc;
using System;

namespace Blog.Middlewares
{
    public enum CacheMode
    {
        USER_SCOPED = 0,
        PUBLIC,
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CustomResponseCacheAttribute : Attribute
    {
        public int ClientCacheDuration { get; }
        public int ServerCacheDuration { get; }
        public CacheMode Mode { get; }

        public CustomResponseCacheAttribute(int clientCacheDuration, int serverCacheDuration, CacheMode cacheMode)
        {
            ClientCacheDuration = clientCacheDuration;
            ServerCacheDuration = serverCacheDuration;
            Mode = cacheMode;
        }
    }
}
