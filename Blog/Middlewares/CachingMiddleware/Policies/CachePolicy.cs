using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Middlewares.CachingMiddleware.Policies
{
    public static class CachePolicy
    {
        public const string ANY_USER_SCOPED = nameof(AnyUserScopedCachePolicy);
        public const string AUTHORIZED_USER_SCOPED = nameof(AuthorizedUserScopedCachePolicy);
        public const string UNATHORZED_USER_SCOPED = nameof(UnathorizedUserScopedCachePolicy);
    }
}
