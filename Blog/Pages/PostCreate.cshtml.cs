using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Blog.Pages
{
    public class PostCreateModel : PostCRUDPageModel
    {
        public PostCreateModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task OnGetAsync()
        {
            await S.Permissions.ValidateCreatePostAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await S.Permissions.ValidateCreatePostAsync();

            if (ModelState.IsValid)
            {
                if (await S.Db.Posts.FirstOrDefaultAsync(p => p.Title == Title) != null)
                {
                    ModelState.AddModelError("", "Post with this name already exists");

                    return Page();
                }
                else
                {
                    var author = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                    var body = await getEscapedPostBodyAsync();
                    var preview = getPostBodyPreview(body);
                    var post = new Post(DateTime.UtcNow, author, Title, body, preview);
                    S.Db.Posts.Add(post);
                    if (await S.Permissions.CanCreatePostsWithoutModerationAsync())
                    {
                        await S.Moderation.MarkPostAsModeratedAsync(post);
                    }
                    else
                    {
                        post.ModerationInfo.State = ModerationState.UNDER_MODERATION;
                        post.Author.ModeratorsInChargeGroup.AddEntityToCheck(post, CheckReason.NEED_MODERATION);
                    }
                    await S.Repository.AddUserActionAsync(author, new UserAction(ActionType.POST_CREATED, post));
                    await S.Db.SaveChangesAsync();

                    return RedirectToPage("/Post", new { id = post.Id });
                }
            }
            else
            {
                return Page();
            }
        }
    }
}