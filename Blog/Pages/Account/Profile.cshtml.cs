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
        User _user;

        public ProfileModel(BlogContext db, AutentificationService autentification)
        {
            _db = db;
            _autentification = autentification;
        }

        public string Username => _user.Nickname;
        public string EMail => _user.EMail;
        public DateTime RegistrationDate => _user.RegistrationDate;
        public IEnumerable<Post> Posts => _db.Posts.Where(p => p.Author == _user);

        public async Task<IActionResult> OnGetAsync()
        {
            _user = await _autentification.GetCurrentUserAsync(HttpContext);

            return Page();
        }
    }
}