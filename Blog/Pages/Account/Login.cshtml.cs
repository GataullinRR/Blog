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

        public LoginModel(ServicesProvider serviceProvider) : base(serviceProvider)
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
                var user = await Services.UserManager.FindByNameAsync(Login);
                if (user == null)
                {
                    ModelState.AddModelError("", "This login is not registered");

                    return Page();
                }
                else
                {
                    var result = await Services.SignInManager.PasswordSignInAsync(user, Password, true, false);
                    if (result.Succeeded)
                    {
                        user.Actions.Add(new UserAction(ActionType.SIGNED_IN, null));
                        await Services.Db.SaveChangesAsync();

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        user.Actions.Add(new UserAction(ActionType.SIGN_IN_FAIL, null));
                        await Services.Db.SaveChangesAsync();

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