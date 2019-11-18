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
        public UtilitiesService(ServicesProvider services) : base(services)
        {

        }

        public async Task<User> GetCurrentUserModelOrThrowAsync()
        {
            var user = await Services.UserManager.GetUserAsync(Services.HttpContext.User);
            if (user == null)
            {
                throw new AuthenticationException();
            }
            else
            {
                return user;
            }
        }

        public async Task<User> FindUserByIdOrGetCurrentOrThrowAsync(string userId)
        {
            var user = await Services.UserManager.FindByIdAsync(userId);
            if (user == null)
            {
                return await GetCurrentUserModelOrThrowAsync();
            }

            return user;
        }
    }
}
