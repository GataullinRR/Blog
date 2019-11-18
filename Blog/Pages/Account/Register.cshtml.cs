using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Blog.Models;
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
    public class RegisterModel : PageModelBase
    {
        [Required, MaxLength(16), BindProperty]
        public string Username { get; set; }
        [Required, DataType(DataType.EmailAddress), BindProperty]
        public string EMail { get; set; }
        [Required, DataType(DataType.Password), BindProperty]
        public string Password { get; set; }
        // Compare does not seem to work with BindPropertyAttr
        [DataType(DataType.Password), BindProperty]
        public string ConfirmPassword { get; set; }
        public string RegistrationRole { get; private set; }

        public RegisterModel(ServicesProvider services) : base(services)
        {

        }

        public IActionResult OnGet()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("/Account/Logout");
            }
            else
            {
                RegistrationRole = Services.MutatorsManager.RegistrationRole;
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
                var user = await Services.UserManager.FindByNameAsync(Username);
                if (user == null)
                {
                    var newUser = new User(new Profile(DateTime.UtcNow), new ProfileStatus(default))
                    {
                        UserName = Username,
                        Email = EMail
                    };
                    var result = await Services.UserManager.CreateAsync(newUser, Password);
                    if (result.Succeeded)
                    {
                        await Services.UserManager.AddToRoleAsync(newUser, Services.MutatorsManager.RegistrationRole);
                        Services.MutatorsManager.Reset(nameof(Services.MutatorsManager.RegistrationRole));
                        await Services.SignInManager.SignInAsync(newUser, true);
                        await Services.Db.SaveChangesAsync();

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