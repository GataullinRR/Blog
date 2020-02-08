using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using DBModels;

namespace Blog
{
    public class PostDeleteConfirmationModel : PageModelBase
    {
        [Required, BindProperty, MinLength(10), MaxLength(1000)]
        public string Reason { get; set; }
        [Required, BindProperty]
        public int PostId { get; set; }

        public PostDeleteConfirmationModel(ServiceLocator services) : base(services)
        {

        }

        public async Task<IActionResult> OnGet([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateDeletePostAsync(post);
                PostId = id;

                return Page();
            }
            else
            {
                throw new ArgumentException();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(PostId);
                await S.Permissions.ValidateDeletePostAsync(post);

                post.IsDeleted = true;
                post.DeleteReason = Reason;
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.POST_DELETED, post));
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = PostId });
            }
            else
            {
                return Page();
            }
        }
    }
}