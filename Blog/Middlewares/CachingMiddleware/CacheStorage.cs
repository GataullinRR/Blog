using Utilities.Extensions;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace Blog.Middlewares
{
    class CacheStorage : ICacheStorage
    {
        readonly Dictionary<CacheIdentity, List<CacheEntry>> _cache = new Dictionary<CacheIdentity, List<CacheEntry>>();
        readonly SemaphoreSlim _locker = new SemaphoreSlim(1);

        public IEnumerable<CacheEntry> Entries => _cache.Select(kvp => kvp.Value).Flatten();

        public Task<IDisposable> LockAsync() => _locker.AcquireAsync();

        public async Task TryAddAsync(CacheIdentity id, CacheEntry entry)
        {
            using (await _locker.AcquireAsync())
            {
                _cache.EnsureKeyExists(id, new List<CacheEntry>());
                _cache[id].Add(entry);
            }
        }

        public async Task<IList<CacheEntry>> TryGetAsync(CacheIdentity id)
        {
            using (await _locker.AcquireAsync())
            {
                if (_cache.ContainsKey(id))
                {
                    return _cache[id];
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task TryRemoveAsync(CacheIdentity id)
        {
            using (await _locker.AcquireAsync())
            {
                _cache.Remove(id);
            }
        }

        public async Task TryRemoveAsync(CacheEntry entry)
        {
            using (await _locker.AcquireAsync())
            {
                foreach (var entries in _cache.Values)
                {
                    entries.Remove(entry);
                }
            }
        }

        public async Task TryRemoveAsync(string cacheKey)
        {
            using (await _locker.AcquireAsync())
            {
                foreach (var entries in _cache.Values)
                {
                    entries.RemoveAll(e => e.CachingInfo.CacheEntryKey == cacheKey);
                }
            }
        }

        public async Task TryRemoveAsync(string cacheKey, params KeyValuePair<string, object>[] routeData)
        {
            using (await _locker.AcquireAsync())
            {
                foreach (var kvp in _cache)
                {
                    kvp.Value.RemoveAll(e => 
                        e.CachingInfo.CacheEntryKey == cacheKey && 
                        routeData.All(rd => e.RouteData.Contains(rd)));
                }
            }
        }
    }
}
