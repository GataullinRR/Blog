using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages.Account
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class PasswordChangeModel : ExtendedPageModel
    {
        [BindProperty, DataType(DataType.Password), Required]
        public string NewPassword { get; set; }
        [BindProperty, DataType(DataType.Password), Required]
        public string NewPasswordConfirmation { get; set; }
        [BindProperty, DataType(DataType.Password), Required]
        public string CurrentPassword { get; set; }

        public PasswordChangeModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            PersistLayoutModel = true;
        }

        public async Task OnGetAsync()
        {
            await Permissions.ValidateChangePasswordAsync(await UserManager.GetUserAsync(User));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (NewPassword == NewPasswordConfirmation)
                {
                    var user = await UserManager.GetUserAsync(User);
                    var result = await UserManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignOutAsync();
                        LayoutModel.Messages.Add("Password has been changed");

                        return RedirectToPage("/Account/Login", new { userName = user.UserName });
                    }
                    else
                    { 
                        LayoutModel.Messages.Add("Could not change password");

                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Passwords do not match");

                    return Page();
                }
            }
            else
            {
                return Page();
            }
        }
    }
}