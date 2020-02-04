using System;
using System.Collections.Generic;

namespace Blog.Middlewares
{
    public class CacheScope
    {
        public CacheScope(object data, IServiceProvider serviceProvider)
        {
            RequestData = data ?? throw new ArgumentNullException(nameof(data));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object RequestData { get; }
        public IServiceProvider ServiceProvider { get; }
    }
}
