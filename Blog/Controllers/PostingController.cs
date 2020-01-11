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
        public PostingController(ServicesLocator serviceProvider) : base(serviceProvider)
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
                await S.Permissions.ValidateUndeletePostAsync(post);

                post.IsDeleted = false;
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.POST_UNDELETED, post));
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