using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class UtilitiesService : ServiceBase
    {
        public UtilitiesService(ServiceLocator services) : base(services)
        {

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
            return await S.UserManager.GetUserAsync(S.HttpContext.User);
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
