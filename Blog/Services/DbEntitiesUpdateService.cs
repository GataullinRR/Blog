using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class DbEntitiesUpdateService : ServiceBase
    {
        public DbEntitiesUpdateService(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task EnsureHasEnoughModeratorsInChargeAsync(User user)
        {
            var moderatorsRequired = (2 - user.ModeratorsInCharge.Count).NegativeToZero();
            if (moderatorsRequired > 0)
            {
                var moderatorsToAssign = (await Services.Db.GetUsersInRoleAsync(Roles.MODERATOR))
                    .OrderBy(m => Services.Db.Users.Count(u => u.ModeratorsInCharge.Contains(m)))
                    .Where(m => user.ModeratorsInCharge.NotContains(m))
                    .Where(m => m != user)
                    .Take(moderatorsRequired);
                user.ModeratorsInCharge.AddRange(moderatorsToAssign);

                await Services.Db.SaveChangesAsync();
            }
        }
    }
}
