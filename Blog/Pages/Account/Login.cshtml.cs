using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    public class LoginModel : PageModel
    {
        readonly BlogContext _db;
        readonly AutentificationService _autentification;

        public LoginModel(BlogContext db, AutentificationService autentification)
        {
            _db = db;
            _autentification = autentification;
        }

        [BindProperty]
        [Required(ErrorMessage = "Login is required")]
        public string Login { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
        public string Password { get; set; }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _db.Users.FirstOrDefaultAsync(u => u.Nickname == Login);
                if (user == null)
                {
                    ModelState.AddModelError("", "This login is not registered");

                    return Page();
                }
                else
                {
                    var isLoggedIn = await _autentification
                        .TryAuthenticateAsync(HttpContext, user.Nickname, Password);

                    if (isLoggedIn)
                    {
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login or password is not valid");

                        return Page();
                    }
                }
            }
            else
            {
                return Page();
            }
        }
    }
}