using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Utilities.Types;

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
        public Key CacheEntryKey { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientCacheDuration">Secounds</param>
        /// <param name="serverCacheDuration">Secounds</param>
        /// <param name="cacheMode"></param>
        /// <param name="handle">It'll be called when cache is found</param>
        /// <param name="cacheEntryKey">Unique key used to perform cache operations through <see cref="CacheManagingFeature"/></param>
        public CustomResponseCacheAttribute(int clientCacheDuration, int serverCacheDuration, CacheMode cacheMode, string cacheEntryKey)
        {
            ClientCacheDuration = clientCacheDuration;
            ServerCacheDuration = serverCacheDuration;
            Mode = cacheMode;
            CacheEntryKey = cacheEntryKey;
        }

        public CustomResponseCacheAttribute(int clientCacheDuration, int serverCacheDuration, CacheMode cacheMode)
            : this(clientCacheDuration, serverCacheDuration, cacheMode, null)
        {

        }
    }
}
