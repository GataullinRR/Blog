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
                await S.Moderation.MarkPostAsModeratedAsync(post);

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
                await S.Moderation.MarkPostEditAsModeratedAsync(edit);

                return RedirectToPage("/Post", new { id = edit.Post.Id });
            }
            else
            {
                throw new Exception();
            }
        }
    }
}