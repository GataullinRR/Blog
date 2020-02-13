using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ServerResponseCacheAttribute : Attribute
    {
        public int CacheDuration { get; }
        public string CachePolicy { get; }
        public string CacheEntryKey { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverCacheDuration">Secounds</param>
        /// <param name="cacheEntryKey">Unique key used to perform cache operations through <see cref="CacheManagingFeature"/></param>
        public ServerResponseCacheAttribute(int serverCacheDuration, string cachePolicy, string cacheEntryKey)
        {
            CacheDuration = serverCacheDuration;
            CachePolicy = cachePolicy;
            CacheEntryKey = cacheEntryKey;
        }

        public ServerResponseCacheAttribute(int serverCacheDuration, string cachePolicy)
            : this(serverCacheDuration, cachePolicy, null)
        {

        }
    }
}
