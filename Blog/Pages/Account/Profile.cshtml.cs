using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        public ProfileModel(BlogContext db)
        {
            _db = db;
        }

        public string Username { get; private set; }
        public string EMail { get; private set; }
        public DateTime RegistrationDate { get; private set; }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Nickname == User.Identity.Name);
            if (user == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                Username = user.Nickname;
                EMail = user.EMail;
                RegistrationDate = user.RegistrationDate;

                return Page();
            }
        }
    }
}