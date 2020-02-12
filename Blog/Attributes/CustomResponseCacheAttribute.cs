using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Attributes
{
    /// <summary>
    /// For server-side caching
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CustomResponseCacheAttribute : Attribute
    {
        public int CacheDuration { get; }
        public string CachePolicy { get; }
        public string CacheEntryKey { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverCacheDuration">Secounds</param>
        /// <param name="cacheEntryKey">Unique key used to perform cache operations through <see cref="CacheManagingFeature"/></param>
        public CustomResponseCacheAttribute(int serverCacheDuration, string cachePolicy, string cacheEntryKey)
        {
            CacheDuration = serverCacheDuration;
            CachePolicy = cachePolicy;
            CacheEntryKey = cacheEntryKey;
        }

        public CustomResponseCacheAttribute(int serverCacheDuration, string cachePolicy)
            : this(serverCacheDuration, cachePolicy, null)
        {

        }
    }
}
