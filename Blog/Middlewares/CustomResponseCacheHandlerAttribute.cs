using System;
using Utilities.Types;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    /// <summary>
    /// Must be set on a static method taking <see cref="CacheScope"/> and returning <see cref="Task"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CustomResponseCacheHandlerAttribute : Attribute
    {
        /// <summary>
        /// An unique key, used inside <see cref="CustomResponseCacheAttribute"/>
        /// </summary>
        public Key CacheEntryKey { get; }

        public CustomResponseCacheHandlerAttribute(string cacheEntryKey)
        {
            CacheEntryKey = cacheEntryKey ?? throw new ArgumentNullException();
        }
    }
}
