using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Pages.Account;
using DBModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    public class RegisterModel : PageModel
    {
        readonly BlogContext _db;
        readonly AutentificationService _autentification;

        public RegisterModel(BlogContext db, AutentificationService autentification)
        {
            _db = db;
            _autentification = autentification;
        }

        [Required, MaxLength(16), BindProperty]
        public string Username { get; set; }

        [Required, DataType(DataType.EmailAddress), BindProperty]
        public string EMail { get; set; }

        [Required, DataType(DataType.Password), BindProperty]
        public string Password { get; set; }

        [DataType(DataType.Password), Compare(nameof(Password)), BindProperty]
        public string ConfirmPassword { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("/Account/Logout");
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Nickname == Username);
                if (user == null)
                {
                    _db.Users.Add(new User()
                    {
                        Nickname = Username,
                        RegistrationDate = DateTime.Now,
                        PasswordHash = _autentification.GetHash(Password),
                        EMail = EMail
                    });
                    await _db.SaveChangesAsync();
                    await _autentification.TryAuthenticateAsync(HttpContext, Username, Password);

                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError("", "This username has already been taken");
                }
            }

            return Page();
        }
    }
}