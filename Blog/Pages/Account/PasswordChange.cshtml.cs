using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages.Account
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class PasswordChangeModel : PageModelBase
    {
        [BindProperty, DataType(DataType.Password), Required]
        public string NewPassword { get; set; }
        [BindProperty, DataType(DataType.Password), Required]
        public string NewPasswordConfirmation { get; set; }
        [BindProperty, DataType(DataType.Password), Required]
        public string CurrentPassword { get; set; }

        public PasswordChangeModel(ServiceLocator serviceProvider) : base(serviceProvider)
        {
        }

        public async Task OnGetAsync()
        {
            await S.Permissions.ValidateChangePasswordAsync(await S.UserManager.GetUserAsync(User));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (NewPassword == NewPasswordConfirmation)
                {
                    var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                    await S.Permissions.ValidateChangePasswordAsync(user);
                    await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.PASSWORD_CHANGING, user));
                    var result = await S.UserManager.ChangePasswordAsync(user, CurrentPassword, NewPassword);
                    if (result.Succeeded)
                    {
                        await S.SignInManager.SignOutAsync();
                        user.Actions.Add(new DBModels.UserAction(ActionType.PASSWORD_CHANGED, null));
                        await S.Db.SaveChangesAsync();

                        LayoutModel.AddMessage("Password has been changed");

                        return RedirectToPage("/Account/Login", new { userName = user.UserName });
                    }
                    else
                    {
                        LayoutModel.AddMessage("Could not change password");

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