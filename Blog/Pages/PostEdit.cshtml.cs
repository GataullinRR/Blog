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
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(id);
                if (post != null)
                {
                    await S.Permissions.ValidateEditPostAsync(post);
                    Title = post.Title;
                    Body = post.Body;
                    Post = post;
                    PostId = id;

                    return Page();
                }
            }

            return Redirect(S.History.GetLastURL());
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var editingPost = await S.Db.Posts.FirstOrDefaultByIdAsync(PostId);

                await S.Permissions.ValidateEditPostAsync(editingPost);

                var author = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                editingPost.Body = Body;
                if (await S.Permissions.CanEditPostTitleAsync(editingPost))
                {
                    editingPost.Title = Title;
                }
                editingPost.Edits.Add(new PostEdit(author, EditReason, DateTime.UtcNow));
                author.Actions.Add(new UserAction(ActionType.POST_EDIT, editingPost));
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = editingPost.Id });
            }
            else
            {
                return Page();
            }
        }
    }
}