using Blog.Middlewares;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.HttpContextFeatures
{
    class CacheManagerFeature : ICacheManagerFeature
    {
        readonly ICacheStorage _storage;

        public bool IsRequestDataSet { get; private set; }
        public object RequestData { get; private set; }

        public CacheManagerFeature(ICacheStorage storage, bool isRequestDataSet, object requestData)
        {
            _storage = storage ?? throw new ArgumentNullException(nameof(storage));
            IsRequestDataSet = isRequestDataSet;
            RequestData = requestData;
        }

        public async Task<T> GetRequestDataAsync<T>()
        {
            if (IsRequestDataSet)
            {
                return (T)RequestData;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public Task ResetCacheAsync(string cacheKey)
        {
            return _storage.TryRemoveAsync(cacheKey);
        }

        public Task ResetCacheAsync(string cacheKey, params KeyValuePair<string, object>[] routeData)
        {
            return _storage.TryRemoveAsync(cacheKey, routeData);
        }

        public async Task SetRequestDataAsync<T>(T data)
        {
            if (IsRequestDataSet)
            {
                throw new InvalidOperationException();
            }
            else
            {
                IsRequestDataSet = true;
                RequestData = data;
            }
        }
    }
}
