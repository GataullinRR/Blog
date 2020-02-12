using Blog.HttpContextFeatures;
using Blog.Middlewares;
using Blog.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class CacheManagerService : ServiceBase
    {
        public const string POST_GET_CACHE_KEY = nameof(POST_GET_CACHE_KEY);
        public const string INDEX_GET_CACHE_KEY = nameof(INDEX_GET_CACHE_KEY);

        readonly ICacheManagerFeature _cacheManager;
        
        public ICacheManagerFeature CacheManager => _cacheManager;

        public CacheManagerService(ServiceLocator services) : base(services)
        {
            _cacheManager = services.HttpContext.Features.Get<ICacheManagerFeature>();
        }

        public async Task ResetIndexPageCacheAsync()
        {
            await _cacheManager.ResetCacheAsync(INDEX_GET_CACHE_KEY);
        }
        public async Task ResetPostPageCacheAsync(int postId)
        {
            await _cacheManager.ResetCacheAsync(POST_GET_CACHE_KEY, new KeyValuePair<string, object>("id", postId.ToString()));
        }
    }
}
