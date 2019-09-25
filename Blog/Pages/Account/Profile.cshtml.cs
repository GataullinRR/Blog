using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        readonly BlogContext _db;
        readonly AutentificationService _autentification;
        public User UserModel { get; private set; }

        public ProfileModel(BlogContext db, AutentificationService autentification)
        {
            _db = db;
            _autentification = autentification;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await _autentification.GetCurrentUserAsync(HttpContext);
            UserModel.Posts = await _db.Posts.Where(p => p.Author == UserModel).ToListAsync();

            return Page();
        }
    }
}