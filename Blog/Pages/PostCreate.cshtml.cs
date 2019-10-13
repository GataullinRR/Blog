using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
    public class PostCreateModel : PageModel
    {
        readonly PermissionsService _permissions;
        readonly UserManager<User> _userManager;
        readonly BlogContext _db;

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

        public PostCreateModel(UserManager<User> userManager, BlogContext db, PermissionsService permissions)
        {
            _userManager = userManager;
            _db = db;
            _permissions = permissions;
        }

        public async Task OnGetAsync(int? editingPostId)
        {
            IsEditMode = editingPostId.HasValue;
            if (IsEditMode)
            {
                EditingPostId = editingPostId.Value;
                var editingPost = await _db.Posts
                    .Include(p => p.Edits)
                    .Include(p => p.Author)
                    .FirstOrDefaultAsync(p => p.Id == EditingPostId);
                if (editingPost != null)
                {
                    _permissions.ValidateEditPost(User, editingPost);
                    Title = editingPost.Title;
                    Body = editingPost.Body;
                }
                else
                {
                    throw new ArgumentNullException($"The post with id {editingPostId} does not exists");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            int postId = 0;

            if (IsEditMode)
            {
                var editingPost = await _db.Posts
                    .Include(p => p.Edits)
                    .FirstOrDefaultAsync(p => p.Id == EditingPostId);
                editingPost.Body = Body;
                postId = EditingPostId;
                editingPost.Edits = editingPost.Edits ?? new List<PostEditInfo>();
                editingPost.Edits.Add(new PostEditInfo()
                {
                    Author = await _userManager.GetUserAsync(User),
                    EditTime = DateTime.UtcNow,
                    Reason = EditReason
                });

                await _db.SaveChangesAsync();
            }
            else
            {
                if (await _db.Posts.FirstOrDefaultAsync(p => p.Title == Title) != null)
                {
                    throw new ArgumentOutOfRangeException("The post with this name already exists");
                }

                var post = new Post()
                {
                    Author = await _userManager.GetUserAsync(HttpContext.User),
                    Body = Body,
                    Date = DateTime.Now,
                    Title = Title
                };

                _db.Posts.Add(post);
                await _db.SaveChangesAsync();

                postId = post.Id;
            }

            return RedirectToPage("/Post", new { id = postId });
        }
    }
}