using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages
{
    public class PostEditModel : PostCRUDPageModel
    {
        [BindProperty, Required(), MinLength(10), MaxLength(1000)]
        public string EditReason { get; set; }
        public Post Post { get; private set; }

        public PostEditModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var post = await Services.Db.Posts.FirstOrDefaultByIdAsync(id);
                if (post != null)
                {
                    await Services.Permissions.ValidateEditPostAsync(post);
                    Title = post.Title;
                    Body = post.Body;
                    Post = post;
                    PostId = id;

                    return Page();
                }
            }

            return Redirect(Services.History.GetLastURL());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var editingPost = await Services.Db.Posts.FirstOrDefaultByIdAsync(PostId);

                await Services.Permissions.ValidateEditPostAsync(editingPost);

                var author = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
                editingPost.Body = Body;
                if (await Services.Permissions.CanEditPostTitleAsync(editingPost))
                {
                    editingPost.Title = Title;
                }
                editingPost.Edits.Add(new PostEdit(author, EditReason, DateTime.UtcNow));
                author.Actions.Add(new UserAction(ActionType.POST_EDIT, editingPost));
                await Services.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = editingPost.Id });
            }
            else
            {
                return Page();
            }
        }
    }
}