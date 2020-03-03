using Blog.Attributes;
using Blog.HttpContextFeatures;
using Blog.Middlewares;
using Blog.Misc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;
using ASPCoreUtilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class CacheManagerService : ServiceBase
    {
        public const string POST_GET_CACHE_KEY = nameof(POST_GET_CACHE_KEY);
        public const string INDEX_GET_CACHE_KEY = nameof(INDEX_GET_CACHE_KEY);
        public const string PROFILE_GET_CACHE_KEY = nameof(PROFILE_GET_CACHE_KEY);

        public ICacheManagerFeature CacheManager { get; }

        public CacheManagerService(ServiceLocator services) : base(services)
        {
            CacheManager = services.HttpContext.Features.Get<ICacheManagerFeature>();
        }

        public async Task ResetIndexPageCacheAsync()
        {
            await CacheManager.ResetCacheAsync(INDEX_GET_CACHE_KEY);
        }
        public async Task ResetPostPageCacheAsync(int postId)
        {
            await CacheManager.ResetCacheAsync(POST_GET_CACHE_KEY, new KeyValuePair<string, object>("id", postId.ToString()));

            var targetPost = await S.Db.Posts.AsNoTracking().Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == postId);
            await ResetProfilePageCacheAsync(targetPost.Author.Id);
        }
        public async Task ResetProfilePageCacheAsync(string userId)
        {
            await CacheManager.ResetCacheAsync(PROFILE_GET_CACHE_KEY, new KeyValuePair<string, object>("id", userId));
        }
    }
}
