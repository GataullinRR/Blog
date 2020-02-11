using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DBModels;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Pages
{
    [Authorize]
    public class MarkAsNotPassedModerationModel : PageModelBase
    {
        [BindProperty, BindRequired, Required]
        public int PostId { get; set; }
        [BindProperty, BindRequired, Required, MinLength(8), MaxLength(500)]
        public string Reason { get; set; }

        public MarkAsNotPassedModerationModel(ServiceLocator services) : base(services)
        {

        }

        public async Task OnGet([Required]int id)
        {
            if (ModelState.IsValid)
            {
                PostId = id;
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                await S.Permissions.ValidateMarkAsNotPassedModerationAsync(PostId);
                await S.Moderation.MarkAsNotPassedModerationAsync(PostId, Reason);
                await S.Db.SaveChangesAsync();

                LayoutModel.AddMessage("The post has been marked as not passed moderation");

                return RedirectToPage("/Post", new { id = PostId });
            }
            else
            {
                throw new Exception();
            }
        }

        async Task<IModeratable> getEntityAsync()
        {
            return await S.Db.Posts.FirstOrDefaultByIdAsync(PostId);
        }
    }
}