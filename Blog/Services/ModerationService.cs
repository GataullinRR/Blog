using Blog.Attributes;
using DBModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities.Types;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class ModerationService : ServiceBase
    {
        public ModerationService(ServiceLocator services) : base(services)
        {

        }

        public async Task MarkPostAsModeratedAsync(Post post)
        {
            await markAsync(post);
            await S.Db.SaveChangesAsync();
        }

        public async Task MarkAsNotPassedModerationAsync(int postId, string stateReasoning)
        {
            var post = await S.Db.Posts
                .Include(p => p.ModerationInfo)
                .FirstOrDefaultAsync(p => p.Id == postId);

            post.ModerationInfo.State = ModerationState.MODERATION_NOT_PASSED;
            post.ModerationInfo.StateReasoning = stateReasoning;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.MARKED_AS_NOT_PASSED_MODERATION, post));

            await deleteFromAnyModeratorPanelAsyn(post);
        }

        async Task markAsync(Post moderatable)
        {
            await S.Permissions.ValidateMarkAsModeratedAsync(moderatable);

            moderatable.ModerationInfo.State = ModerationState.MODERATED;
            moderatable.ModerationInfo.StateReasoning = null;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            if (currentUser != moderatable.Author)
            {
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.MARKED_AS_MODERATED, moderatable));
            }

            await deleteFromAnyModeratorPanelAsyn(moderatable);
        }

        async Task deleteFromAnyModeratorPanelAsyn(IModeratable moderatable)
        {
            var checkable = await S.Db.PostsToCheck
                .Where(e => e.Entity.Id == moderatable.Id && e.ResolvingTime == null)
                .ToListAsync();
            foreach (var item in checkable)
            {
                item.ResolvingTime = DateTime.UtcNow; // Delete from every moderators panel
            }
        }
    }
}
