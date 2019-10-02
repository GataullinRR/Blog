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
        readonly UserManager<User> _userManager;
        readonly BlogContext _db;

        [BindProperty]
        public string Title { get; set; }
        [BindProperty]
        public string Body { get; set; }
        [BindProperty]
        public bool IsEditMode { get; set; }
        [BindProperty]
        public int EditingPostId { get; set; }

        public PostCreateModel(UserManager<User> userManager, BlogContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task OnGetAsync(int? editingPostId)
        {
            IsEditMode = editingPostId.HasValue;
            if (IsEditMode)
            {
                EditingPostId = editingPostId.Value;
                var editingPost = await _db.Posts.FirstOrDefaultAsync(p => p.Id == EditingPostId);
                Title = editingPost.Title;
                Body = editingPost.Body;
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateAsync()
        {
            int postId = 0;

            if (IsEditMode)
            {
                var editingPost = await _db.Posts.FirstOrDefaultAsync(p => p.Id == EditingPostId);
                editingPost.Title = Title;
                editingPost.Body = Body;
                postId = EditingPostId;

                await _db.SaveChangesAsync();
            }
            else
            {
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