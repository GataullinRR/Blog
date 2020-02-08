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

        readonly ICacheManagerFeature _cacheManager;
        
        public ICacheManagerFeature CacheManager => _cacheManager;

        public CacheManagerService(ServiceLocator services) : base(services)
        {
            _cacheManager = services.HttpContext.Features.Get<ICacheManagerFeature>();
        }

        public async Task ResetPostCacheAsync()
        {
            await _cacheManager.ResetCacheAsync(POST_GET_CACHE_KEY);
        }

    }
}
