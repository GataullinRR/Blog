using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class IndexModel : PageModel
    {
        readonly BlogContext _db;

        public IEnumerable<Post> Posts => _db.Posts.Include(p => p.Author);

        public IndexModel(BlogContext db)
        {
            _db = db;
        }

        public void OnGet()
        {

        }
    }
}