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
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class PostCreateModel : ExtendedPageModel
    {
        [BindProperty(), Required(), MinLength(8), MaxLength(100)]
        public string Title { get; set; }
        [BindProperty(), Required(), MinLength(500), MaxLength(100000)]
        public string Body { get; set; }
        [BindProperty]
        public bool IsEditMode { get; set; }
        [BindProperty]
        public int EditingPostId { get; set; }
        [BindProperty, Required(), MinLength(10), MaxLength(1000)]
        public string EditReason { get; set; }

        public PostCreateModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task OnGetAsync(int? editingPostId)
        {
            IsEditMode = editingPostId.HasValue;
            if (IsEditMode)
            {
                EditingPostId = editingPostId.Value;
                var editingPost = await DB.Posts
                    .Include(p => p.Edits)
                    .Include(p => p.Author)
                    .FirstOrDefaultAsync(p => p.Id == EditingPostId);
                if (editingPost != null)
                {
                    await Permissions.ValidateEditPostAsync(editingPost);
                    Title = editingPost.Title;
                    Body = editingPost.Body;
                }
                else
                {
                    throw new ArgumentNullException($"The post with id {editingPostId} does not exists");
                }
            }
        }

#warning no model state check
        [HttpPost]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            int postId = 0;

            var author = await GetCurrentUserModelOrThrowAsync();
            if (IsEditMode)
            {
                var editingPost = await DB.Posts
                    .Include(p => p.Edits)
                    .FirstOrDefaultAsync(p => p.Id == EditingPostId);
                editingPost.Body = Body;
                postId = EditingPostId;
                editingPost.Edits.Add(new PostEdit(author, EditReason, DateTime.UtcNow));
                author.Actions.Add(new UserAction(ActionType.POST_EDIT, postId.ToString()));

                await DB.SaveChangesAsync();
            }
            else
            {
                if (await DB.Posts.FirstOrDefaultAsync(p => p.Title == Title) != null)
                {
                    throw new ArgumentOutOfRangeException("The post with this name already exists");
                }

                var post = new Post(DateTime.UtcNow, author, Title, Body);
                DB.Posts.Add(post);
                author.Actions.Add(new UserAction(ActionType.POST_CREATE, post.Id.ToString()));
                await DB.SaveChangesAsync();

                postId = post.Id;
            }

            return RedirectToPage("/Post", new { id = postId });
        }
    }
}