using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class ModerationController : ControllerBase
    {
        public ModerationController(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> MarkPostAsModeratedAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(id);
                await markAsync(post);
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = id });
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> MarkPostEditAsModeratedAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var edit = await S.Db.PostsEdits.FirstOrDefaultByIdAsync(id);
                edit.Post.Title = edit.NewTitle;
                edit.Post.Body= edit.NewBody;
                edit.Post.BodyPreview = edit.NewBodyPreview;
                await markAsync(edit);
                foreach (var editToClose in edit.Post.Edits.Where(e => e.EditTime < edit.EditTime))
                {
                    if (await S.Permissions.CanMarkAsModeratedAsync(editToClose))
                    {
                        await markAsync(editToClose);
                    }
                }
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = edit.Post.Id });
            }
            else
            {
                throw new Exception();
            }
        }

        async Task markAsync(IModeratable moderatable)
        {
            await S.Permissions.ValidateMarkAsModeratedAsync(moderatable);
            moderatable.State = ModerationState.MODERATED;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            currentUser.Actions.Add(new UserAction(ActionType.MARKED_AS_MODERATED, moderatable));
            var checkable = S.Db.EntitiesToCheck.FirstOrDefault(e => e.Entity == moderatable);
            if (checkable != null)
            {
                checkable.ResolvingTime = DateTime.UtcNow; // Delete from every moderators panel
            }
        }
    }
}