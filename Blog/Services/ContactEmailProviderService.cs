﻿using Blog.Misc;
using DBModels;
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

        public ContactEmailProviderService(ServicesLocator services) : base(services)
        {

        }

        public async Task<string> GetHelpContactEmailAsync()
        {
            var currentUser = await S.Utilities.GetCurrentUserModelAsync();
            string email = null;
            if (currentUser != null)
            {
                var moderators = currentUser.ModeratorsInChargeGroup.Moderators
                    .Where(m => m.EmailConfirmed)
                    .ToArray();
                var moderator = _rnd.NextElementFrom(moderators);
                email = moderator.Email;
            }
            else
            {
                var moderators = await S.Db.GetUsersInRoleAsync(Roles.MODERATOR).ThenDo(r => r
                    .Where(m => m.EmailConfirmed)
                    .ToArray());
                var moderator = _rnd.NextElementFrom(moderators);
                email = moderator.Email;
            }

            return email;
        }
    }
}