using Blog.Attributes;
using Blog.Misc;
using DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class ContactEmailProviderService : ServiceBase
    {
        readonly Random _rnd = new Random();

        public ContactEmailProviderService(ServiceLocator services) : base(services)
        {

        }

        public async Task<string> GetHelpContactEmailAsync()
        {
            var currentUserQuery = await S.Utilities.GetCurrentUserAsQueryableAsync();
            string email = null;
            if (currentUserQuery != null)
            {
                var currentUser = await currentUserQuery
                    .Include(u => u.ModeratorsInChargeGroup)
                    .ThenInclude(g => g.Moderators)
                    .AsNoTracking()
                    .SingleAsync();
                if (currentUser.ModeratorsInChargeGroup == null)
                {
                    return null;
                }

                var moderators = currentUser.ModeratorsInChargeGroup.Moderators
                    .Where(m => m.EmailConfirmed)
                    .ToArray();
                var moderator = _rnd.NextElementFrom(moderators);
                email = moderator.Email;
            }
            else
            {
                var moderators = await S.Db.Users.Where(u => u.Role == Role.MODERATOR && u.EmailConfirmed)
                    .AsNoTracking()
                    .ToListAsync();
                var moderator = _rnd.NextElementFromList(moderators);
                email = moderator.Email;
            }

            return email;
        }
    }
}
