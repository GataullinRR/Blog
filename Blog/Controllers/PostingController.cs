using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize]
    public class PostingController : ControllerBase
    {
        public PostingController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [HttpPost]
        public async Task<string> GetPostBodyPreviewAsync([Required]string rawBody)
        {
            if (ModelState.IsValid)
            {
                return await S.Sanitizer.SanitizePostBodyAsync(rawBody);
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> UndeletePostAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateRestorePostAsync(post);

                post.IsDeleted = false;
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.POST_RESTORED, post));
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = id });
            }
            else
            {
                throw new Exception();
            }
        }
    }
}