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
                    var post = new Post(DateTime.UtcNow, author, Title, Body);
                    S.Db.Posts.Add(post);
                    await S.Db.SaveChangesAsync();
                    author.Actions.Add(new UserAction(ActionType.POST_CREATE, post));
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