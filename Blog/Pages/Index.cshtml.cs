using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class IndexModel : ExtendedPageModel
    {
        readonly BlogContext _db;

        public UserManager<User> UserManager { get; }
        public IEnumerable<Post> Posts => _db.Posts.Include(p => p.Author);

        public IndexModel(BlogContext db, UserManager<User> userManager)
        {
            _db = db;
            UserManager = userManager;
        }

        public void OnGet()
        {

        }
    }
}