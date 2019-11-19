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
    public class PasswordRestoreModel : PageModelBase
    {
        readonly EMailService _email;
        readonly ConfirmationLinksGeneratorService _confirmation;

        [BindProperty(), Required()]
        public string UserName { get; set; }

        public PasswordRestoreModel(EMailService email, ConfirmationLinksGeneratorService confirmation, ServicesProvider serviceProvider) : base(serviceProvider)
        {
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _confirmation = confirmation ?? throw new ArgumentNullException(nameof(confirmation));
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await S.UserManager.FindByNameAsync(UserName);
                if (user == null)
                {
                    ModelState.AddModelError("", $"Profile \"{UserName}\" does not exist");

                    return Page();
                }
                else
                {
                    await S.Permissions.ValidateResetPasswordAsync(user);
                    user.Actions.Add(new UserAction(ActionType.PASSWORD_RESETING, user));

                    var link = await _confirmation.GetPasswordResetConfirmationLinkAsync(user);
                    var isSent = await _email.TrySendMessageAsync(user, "Password reset", "Confirmation", $@"If you want to continue password reset, follow this link: {link}
After openning the link, new password will be sent to this E-Mail");
                    if (isSent)
                    {
                        user.Status.LastPasswordRestoreAttempt = DateTime.UtcNow;
                        await S.UserManager.UpdateAsync(user);

                        LayoutModel.AddMessage("Password reset confirmation link has been sent");

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        LayoutModel.AddMessage("Error while sending pasword reset confirmation link");

                        return Page();
                    }
                }
            }
            else
            {
                LayoutModel.AddMessage("Profile name is not provided");

                return Page();
            }
        }
    }
}