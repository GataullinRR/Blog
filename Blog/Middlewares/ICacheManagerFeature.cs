using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Middlewares
{
    public interface ICacheManagerFeature
    {
        Task ResetCacheAsync(Key cacheKey);
        /// <summary>
        /// Is meant to be used to store route parameters
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cacheKey"></param>
        /// <param name="dataKey"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        Task SetRequestDataAsync<T>(Key cacheKey, T data) where T : class;
        Task<T> GetRequestDataAsync<T>(Key cacheKey) where T : class;
    }
}