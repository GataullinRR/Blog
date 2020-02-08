using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages.Account
{
    public class EmailChangeModel : PageModelBase
    {
        [BindProperty, Required, DataType(DataType.Password)]
        public string Password { get; set; }
        [BindProperty, Required, DataType(DataType.EmailAddress)]
        public string NewEMail { get; set; }

        public EmailChangeModel(ServiceLocator services) : base(services)
        {

        }

        public async Task OnGet()
        {
            var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            await S.Permissions.ValidateChangeEmailAsync(user);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                autoSaveDbChanges = true;

                var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                await S.Permissions.ValidateChangeEmailAsync(user);

                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.EMAIL_CHANGING, user));

                if (NewEMail.ToUpperInvariant() == user.NormalizedEmail)
                {
                    ModelState.AddModelError("", "Enter new e-mail");

                    return Page();
                }
                var isPasswordValid = await S.UserManager.CheckPasswordAsync(user, Password);
                if (!isPasswordValid)
                {
#warning security problem
                    ModelState.AddModelError("", "Incorrect password");

                    return Page();
                }

                var confirmationLink = await S.ConfirmationLinks.GetEMailChangeConfirmationLinkAsync(user, NewEMail);
                var isSent = await S.EMail.TrySendMessageAsync(user, "E-Mail change", "Administration", $@"You are trying to change profile e-mail.
Current e-mail: {user.Email}
New e-mail: {NewEMail}
Follow this link to finish the operation: {confirmationLink}");

                if (isSent)
                {
                    LayoutModel.AddMessage($"Conformation link has been send to {NewEMail}");

                    return Redirect(S.History.GetLastURL());
                }
                else
                {
                    LayoutModel.AddMessage("Couldn't send confirmation link. Try later.");

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
