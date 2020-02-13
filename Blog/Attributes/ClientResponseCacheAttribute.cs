using System;

namespace Blog.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class ClientResponseCacheAttribute : Attribute
    {
        public int CacheDuration { get; }

        public ClientResponseCacheAttribute(int serverCacheDuration)
        {
            CacheDuration = serverCacheDuration;
        }
    }
}
