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
            var performer = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            await S.Permissions.ValidateBanUserAsync(targetUser);

            if (await S.UserManager.IsInRoleAsync(targetUser, Roles.MODERATOR))
            {
                foreach (var entity in targetUser.ModeratorsGroup.EntitiesToCheck)
                {
                    if (!entity.IsResolved && entity.AssignedModerator == targetUser)
                    {
                        entity.AssignationTime = null;
                        entity.AssignedModerator = null;
                    }
                }
                // Sets ModeratorPanel to null
                //targetUser.ModeratorPanel.Owners.Remove(targetUser);
            }
            targetUser.Status.State = ProfileState.BANNED;
            targetUser.Status.BannedTill = bannedTill.ToUniversalTime();
            targetUser.Status.StateReason = reason;
            await S.Repository.AddUserActionAsync(performer, new UserAction(ActionType.BANNED, targetUser));
            await S.Db.SaveChangesAsync();

            await S.EMail.TrySendMessageAsync(targetUser, "Administration", $"Profile has been banned{isForeverlyBanned.Ternar(" foreverly", "")}", $@"Profile name: {targetUser.UserName}
Reason: {reason}
{isForeverlyBanned.Ternar("", $"Ban will expire at {bannedTill.ToString("dd.MM.yyyy")}")}");
        }

        public async Task UnbanAsync(User targetUser)
        {
            var performer = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            await S.Permissions.ValidateUnbanUserAsync(targetUser);

            targetUser.Status.State = targetUser.EmailConfirmed
                ? ProfileState.ACTIVE
                : ProfileState.RESTRICTED;
            targetUser.Status.StateReason = null;
            targetUser.Status.BannedTill = null;
            await S.Repository.AddUserActionAsync(performer, new UserAction(ActionType.UNBANED, targetUser));
            await S.Db.SaveChangesAsync();
        }
    }
}
