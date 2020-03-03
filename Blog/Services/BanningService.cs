using Blog.Attributes;
using DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;
using ASPCoreUtilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class BanningService : ServiceBase
    {
        public BanningService(ServiceLocator services) : base(services)
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
            var action = new UserAction(ActionType.BANNED, targetUser) { Author = performer };
            S.Db.UsersActions.Add(action);
            await S.Db.SaveChangesAsync();

            await S.EMail.TrySendMessageAsync(targetUser, "Administration", $"Profile has been banned{isForeverlyBanned.Ternar(" foreverly", "")}", $@"Profile name: {targetUser.UserName}
Reason: {reason}
{isForeverlyBanned.Ternar("", $"Ban will expire at {bannedTill.ToString("dd.MM.yyyy")}")}");
        }

        public async Task UnbanAsync(string targetUserId)
        {
            await S.Permissions.ValidateUnbanUserAsync(targetUserId);
            var performer = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            var targetUser = await S.Db.Users
                .Include(u => u.Status)
                .FirstOrDefaultAsync(u => u.Id == targetUserId);

            targetUser.Status.State = targetUser.EmailConfirmed
                ? ProfileState.ACTIVE
                : ProfileState.RESTRICTED;
            targetUser.Status.StateReason = null;
            targetUser.Status.BannedTill = null;
            S.Db.UsersActions.Add(new UserAction(ActionType.UNBANED, targetUser, performer));
            await S.Db.SaveChangesAsync();
        }
    }
}
