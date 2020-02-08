using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
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

        public async Task MarkAsNotPassedModerationAsync(IModeratable moderatable, string stateReasoning)
        {
            await markNotPassedModerationAsync(moderatable);
            moderatable.ModerationInfo.StateReasoning = stateReasoning;
            await S.Db.SaveChangesAsync();
        }

        //public async Task MarkPostEditAsModeratedAsync(PostEdit edit)
        //{
        //    edit.Post.Title = edit.NewTitle;
        //    edit.Post.Body = edit.NewBody;
        //    edit.Post.BodyPreview = edit.NewBodyPreview;
        //    if (await S.Permissions.CanMarkAsModeratedAsync(edit.Post))
        //    {
        //        await markAsync(edit.Post); // Because edit body becomes post body and if this edit's body is corect so the post will also be correct
        //    }
        //    await markAsync(edit);
        //    foreach (var editToClose in edit.Post.Edits.Where(e => e.EditTime < edit.EditTime))
        //    {
        //        if (await S.Permissions.CanMarkAsModeratedAsync(editToClose))
        //        {
        //            await markAsync(editToClose);
        //        }
        //    }
        //    await S.Db.SaveChangesAsync();
        //}

        async Task markAsync(IModeratable moderatable)
        {
            await S.Permissions.ValidateMarkAsModeratedAsync(moderatable);

            moderatable.ModerationInfo.State = ModerationState.MODERATED;
            moderatable.ModerationInfo.StateReasoning = null;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            if (currentUser != moderatable.Author)
            {
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.MARKED_AS_MODERATED, moderatable));
            }

            deleteFromAnyModeratorPanel(moderatable);
        }

        async Task markNotPassedModerationAsync(IModeratable moderatable)
        {
            await S.Permissions.ValidateMarkAsNotPassedModerationAsync(moderatable);

            moderatable.ModerationInfo.State = ModerationState.MODERATION_NOT_PASSED;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.MARKED_AS_NOT_PASSED_MODERATION, moderatable));

            deleteFromAnyModeratorPanel(moderatable);
        }

        void deleteFromAnyModeratorPanel(IModeratable moderatable)
        {
            var checkable = S.Db.EntitiesToCheck.FirstOrDefault(e => e.Entity == moderatable);
            if (checkable != null)
            {
                checkable.ResolvingTime = DateTime.UtcNow; // Delete from every moderators panel
            }
        }
    }
}
