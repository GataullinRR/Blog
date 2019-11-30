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
    public class PostingController : ControllerBase
    {
        public PostingController(ServicesProvider serviceProvider) : base(serviceProvider)
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

        public async Task<IActionResult> DeletePostAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateDeletePostAsync(post);
                post.IsDeleted = true;
                currentUser.Actions.Add(new UserAction(ActionType.POST_DELETED, post));
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