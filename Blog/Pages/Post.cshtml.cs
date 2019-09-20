using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class PostModel : PageModel
    {
        readonly BlogContext _db;

        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }

        public PostModel(BlogContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await _db.Posts.FindAsync(id);

            if (Post == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                Commentaries = _db.Commentaries
                    .Include(c => c.Post)
                    .Where(c => c.Post == Post)
                    .Include(c => c.Author);

                return Page();
            }
        }
    }
}