using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class BanningService : ServiceBase
    {
        public BanningService(ServicesProvider services) : base(services)
        {

        }

        public async Task BanForeverAsync(User targetUser, string reason)
        {
            await BanAsync(targetUser, DateTime.MaxValue, reason);
        }

        public async Task BanAsync(User targetUser, DateTime bannedTill, string reason)
        {
            var isForeverlyBanned = bannedTill == DateTime.MaxValue;
            var performer = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
            await Services.Permissions.ValidateBanUserAsync(targetUser);

            if (await Services.UserManager.IsInRoleAsync(targetUser, Roles.MODERATOR))
            {
                foreach (var entity in targetUser.ModeratorPanel.EntitiesToCheck)
                {
                    if (!entity.IsResolved && entity.AssignedModerator == targetUser)
                    {
                        entity.AssignationTime = null;
                        entity.AssignedModerator = null;
                    }
                } 
                targetUser.ModeratorPanel.Owners.Remove(targetUser);
            }
            targetUser.Status.State = ProfileState.BANNED;
            targetUser.Status.BannedTill = bannedTill.ToUniversalTime();
            targetUser.Status.StateReason = reason;
            performer.Actions.Add(new UserAction(ActionType.BAN, targetUser));
            await Services.Db.SaveChangesAsync();

            await Services.EMail.TrySendMessageAsync(targetUser, "Administration", $"Profile has been banned{isForeverlyBanned.Ternar(" foreverly", "")}", $@"Profile name: {targetUser.UserName}
Reason: {reason}
{isForeverlyBanned.Ternar("", $"Ban will expire at {bannedTill.ToString("dd.MM.yyyy")}")}");
        }
    }
}
