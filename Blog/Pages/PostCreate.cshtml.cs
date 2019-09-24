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

        public void OnGet()
        {

        }

        [HttpPost]
        public async Task<IActionResult> OnPostCreateAsync()
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

            return RedirectToPage("/Post", new { id = post.Id });
        }
    }
}