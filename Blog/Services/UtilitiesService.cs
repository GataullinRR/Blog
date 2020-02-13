using Blog.Attributes;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class UtilitiesService : ServiceBase
    {
        bool _isCurrentUserCacheSet;
        User _curentUserCache;

        public UtilitiesService(ServiceLocator services) : base(services)
        {

        }

        public async Task<string> GetCurrentUserIdOrThrowAsync()
        {
            var user = await GetCurrentUserModelOrThrowAsync();
            return user.Id;
        }

        public async Task<User> GetCurrentUserModelOrThrowAsync()
        {
            var user = await GetCurrentUserModelAsync();
            if (user == null)
            {
                throw new AuthenticationException();
            }
            else
            {
                return user;
            }
        }

        public async Task<User> GetCurrentUserModelAsync()
        {
            if (_isCurrentUserCacheSet)
            {
                return _curentUserCache;
            }

            var user = await S.UserManager.GetUserAsync(S.HttpContext.User);
            _isCurrentUserCacheSet = true;
            _curentUserCache = user;

            return _curentUserCache;
        }

        public async Task<User> FindUserByIdOrGetCurrentOrThrowAsync(string userId)
        {
            var user = await S.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await GetCurrentUserModelOrThrowAsync();
            }

            return user;
        }
    }
}
