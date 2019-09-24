using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    [Authorize]
    public class PostCreateModel : PageModel
    {
        readonly AutentificationService _autentification;
        readonly BlogContext _db;

        public PostCreateModel(AutentificationService autentification, BlogContext db)
        {
            _autentification = autentification;
            _db = db;
        }

        [BindProperty]
        public string Title { get; set; }
        [BindProperty]
        public string Body { get; set; }
        [BindProperty]
        public bool IsEditMode { get; set; }
        [BindProperty]
        public int EditingPostId { get; set; }

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
                    Author = await _autentification.GetCurrentUserAsync(HttpContext),
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