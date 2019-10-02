using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    [Authorize]
    public class ProfileModel : PageModel
    {
        readonly BlogContext _db;
        readonly UserManager<User> _userManager;

        public User UserModel { get; private set; }
        public string Role { get; private set; }

        public ProfileModel(BlogContext db, UserManager<User> userManager)
        {
            _db = db;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await _userManager.GetUserAsync(HttpContext.User);
            UserModel.Posts = await _db.Posts.Where(p => p.Author == UserModel).ToListAsync();
            Role = (await _userManager.GetRolesAsync(UserModel)).Single();

            return Page();
        }
    }
}