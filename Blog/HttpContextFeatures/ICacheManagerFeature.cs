using System.Collections.Generic;
using System.Threading.Tasks;

namespace Blog.HttpContextFeatures
{
    public interface ICacheManagerFeature
    {
        Task ResetCacheAsync(string cacheKey);
        Task ResetCacheAsync(string cacheKey, params KeyValuePair<string, object>[] routeData);
        Task SetRequestDataAsync<T>(T data);
        Task<T> GetRequestDataAsync<T>();
    }
}
