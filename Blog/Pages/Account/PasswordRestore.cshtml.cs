using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    public class PasswordRestoreModel : ExtendedPageModel
    {
        readonly BlogContext _db;
        readonly UserManager<User> _userManager;
        readonly EMailService _email;
        readonly PermissionsService _permissions;
        readonly ConfirmationTokenService _confirmation;

        public PasswordRestoreModel(BlogContext db, UserManager<User> userManager, EMailService email, PermissionsService permissions, ConfirmationTokenService confirmation)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));

            PersistLayoutModel = true;
        }

        [BindProperty(), Required()]
        public string UserName { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", $"Profile \"{UserName}\" does not exist");

                    return Page();
                }
                else
                {
                    _permissions.ValidateResetPassword(User, user);

                    var token = _confirmation.GetToken(user, AccountOperation.PASSWORD_RESET);
                    var link = _confirmation.GenerateLink(HttpContext, token, nameof(AccountController), nameof(AccountController.ConfirmPasswordResetAsync), new { userId = user.Id });
                    var newPassword = Global.Random.NextENWord() + Global.Random.NextENWord() + Global.Random.NextENWord();
                    var isSent = await _email.TrySendMessageAsync(user, "Password reset", "Confirmation", $@"If you want to continue password reset, follow this link: {link}
After openning the link, new password will be sent to this E-Mail");
                    if (isSent)
                    {
                        user.LastPasswordRestoreAttempt = DateTime.UtcNow;
                        await _userManager.UpdateAsync(user);

                        LayoutModel.Messages.Add("Password reset confirmation link has been sent");

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        LayoutModel.Messages.Add("Error while sending pasword reset confirmation link");

                        return Page();
                    }
                }
            }
            else
            {
                LayoutModel.Messages.Add("Profile name is not provided");

                return Page();
            }
        }
    }
}