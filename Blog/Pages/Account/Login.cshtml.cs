using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    public class LoginModel : PageModelBase
    {
        [BindProperty]
        [Required(ErrorMessage = "Login is required")]
        public string Login { get; set; }
        [BindProperty]
        [Required(ErrorMessage = "Password is required"), DataType(DataType.Password)]
        public string Password { get; set; }

        public LoginModel(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        public IActionResult OnGet(string userName)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                Login = userName;

                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await S.UserManager.FindByNameAsync(Login);
                if (user == null)
                {
                    ModelState.AddModelError("", "This login is not registered");

                    return Page();
                }
                else
                {
                    var result = await S.SignInManager.PasswordSignInAsync(user, Password, true, false);
                    if (result.Succeeded)
                    {
                        await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.SIGNED_IN, user));
                        await S.Db.SaveChangesAsync();

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.SIGN_IN_FAILED, user));
                        await S.Db.SaveChangesAsync();

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