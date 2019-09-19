using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages
{
    [Authorize]
    public class PostModel : PageModel
    {
        readonly BlogContext _db;

        public DBModels.Post Post { get; private set; }

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
                return Page();
            }
        }
    }
}