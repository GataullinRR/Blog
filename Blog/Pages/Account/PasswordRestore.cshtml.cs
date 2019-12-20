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

        [BindProperty(), Required(ErrorMessage = "Profile name is not provided")]
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
                var targetUser = await S.UserManager.FindByNameAsync(UserName);
                if (targetUser == null || !await S.Permissions.CanResetPasswordAsync(targetUser))
                {
                    return RedirectToPage("/Errors/PasswordResetError");
                }
                else
                {
                    targetUser.Actions.Add(new UserAction(ActionType.PASSWORD_RESETING, targetUser));
                    var link = await _confirmation.GetPasswordResetConfirmationLinkAsync(targetUser);
                    var isSent = await _email.TrySendMessageAsync(targetUser, "Password reset", "Confirmation", $@"If you want to continue password reset, follow this link: {link}
After openning the link, new password will be sent to this E-Mail");
                    if (isSent)
                    {
                        targetUser.Status.LastPasswordRestoreAttempt = DateTime.UtcNow;
                        await S.UserManager.UpdateAsync(targetUser);

                        LayoutModel.AddMessage("Password reset confirmation link has been sent");

                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        LayoutModel.AddMessage("Error while sending confirmation link");

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