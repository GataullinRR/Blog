using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Pages.Account;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blog.Pages.Account
{
    public class RegisterModel : PageModel
    {
        readonly BlogContext _db;
        readonly SignInManager<User> _signInManager;
        readonly RoleManager<IdentityRole> _roleManager;
        readonly UserManager<User> _userManager;

        [Required, MaxLength(16), BindProperty]
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress), BindProperty]
        public string EMail { get; set; }
        [Required, DataType(DataType.Password), BindProperty]
        public string Password { get; set; }
        // Compare does not seem to work with BindPropertyAttr
        [DataType(DataType.Password), BindProperty]
        public string ConfirmPassword { get; set; }

        public RegisterModel(BlogContext db, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager, UserManager<User> userManager)
        {
            _db = db;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _userManager = userManager;
        }

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
            if (Password != ConfirmPassword)
            {
                ModelState.AddModelError("", "Passwords do not match.");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(Username);
                if (user == null)
                {
                    var newUser = new User()
                    {
                        UserName = Username,
                        RegistrationDate = DateTime.Now,
                        Email = EMail
                    };
                    var result = await _userManager.CreateAsync(newUser, Password);
                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(newUser, Roles.UNCONFIRMED);
                        await _signInManager.SignInAsync(newUser, true);

                        return RedirectToPage("/Account/ConfirmEmail");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Could not create the user. Unknown error.");
                    }
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