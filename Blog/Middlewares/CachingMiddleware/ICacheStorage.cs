using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.Middlewares
{
    interface ICacheStorage
    {
        IEnumerable<CacheEntry> Entries { get; }
        Task<IList<CacheEntry>> TryGetAsync(CacheIdentity id);
        Task TryAddAsync(CacheIdentity id, CacheEntry entry);
        Task TryRemoveAsync(CacheIdentity id);
        Task TryRemoveAsync(CacheEntry entry);
        Task TryRemoveAsync(string cacheKey);
        Task TryRemoveAsync(string cacheKey, params KeyValuePair<string, object>[] routeData);
        Task<IDisposable> LockAsync();
    }
}
