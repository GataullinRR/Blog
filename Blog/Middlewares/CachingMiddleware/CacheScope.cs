using System;
using System.Collections.Generic;

namespace Blog.Middlewares
{
    public class CacheScope
    {
        public CacheScope(bool isRequestDataSet, object data, IServiceProvider serviceProvider)
        {
            IsRequestDataSet = isRequestDataSet;
            RequestData = data;
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public bool IsRequestDataSet { get; }
        public object RequestData { get; }
        public IServiceProvider ServiceProvider { get; }
    }
}
