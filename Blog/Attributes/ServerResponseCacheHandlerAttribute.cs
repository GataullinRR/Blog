using System;
using Utilities.Types;
using System.Threading.Tasks;

namespace Blog.Attributes
{
    /// <summary>
    /// Must be set on a static method taking <see cref="CacheScope"/> and returning <see cref="Task"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ServerResponseCacheHandlerAttribute : Attribute
    {
        /// <summary>
        /// An unique key, used inside <see cref="ServerResponseCacheAttribute"/>
        /// </summary>
        public string CacheEntryKey { get; }

        public ServerResponseCacheHandlerAttribute(string cacheEntryKey)
        {
            CacheEntryKey = cacheEntryKey ?? throw new ArgumentNullException();
        }
    }
}
