using Utilities.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;

namespace Blog.Middlewares
{
    class CacheCleaner
    {
        readonly int _maxCacheSize;
        readonly ICacheStorage _storage;

        public CacheCleaner(int maxCacheSize, ICacheStorage storage)
        {
            _maxCacheSize = maxCacheSize;
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
        }

        async void cleanLoop()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                try
                {
                    CacheEntry[] entries;
                    using (await _storage.LockAsync())
                    {
                        entries = _storage.Entries.ToArray();
                    }

                    var entriesToRemove = new List<CacheEntry>();
                    var actualSize = entries.Sum(e => e.ApproximateSizeInMemory);
                    if (actualSize > _maxCacheSize)
                    {
                        var delta = _maxCacheSize - _maxCacheSize;
                        foreach (var entry in entries.Shake(new Random()))
                        {
                            delta -= entry.ApproximateSizeInMemory;
                            entriesToRemove.Add(entry);

                            if (delta <= 0)
                            {
                                break;
                            }
                        }
                    }

                    foreach (var entry in entriesToRemove)
                    {
                        await _storage.TryRemoveAsync(entry);
                    }
                }
                catch { }
                finally
                {
                    await Task.Delay(1 * 60 * 1000);
                }
            }
        }
    }
}
