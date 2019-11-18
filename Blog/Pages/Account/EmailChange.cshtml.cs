using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
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

        public EmailChangeModel(ServicesProvider services) : base(services)
        {

        }

        public async Task OnGet()
        {
            var user = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
            await Services.Permissions.ValidateChangeEmailAsync(user);
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                autoSaveDbChanges = true;

                var user = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
                await Services.Permissions.ValidateChangeEmailAsync(user);

                user.Actions.Add(new DBModels.UserAction(DBModels.ActionType.EMAIL_CHANGING, user));

                if (NewEMail.ToUpperInvariant() == user.NormalizedEmail)
                {
                    ModelState.AddModelError("", "Enter new e-mail");

                    return Page();
                }
                var isPasswordValid = await Services.UserManager.CheckPasswordAsync(user, Password);
                if (!isPasswordValid)
                {
#warning security problem
                    ModelState.AddModelError("", "Incorrect password");

                    return Page();
                }

                var confirmationLink = await Services.ConfirmationLinks.GetEMailChangeConfirmationLinkAsync(user, NewEMail);
                var isSent = await Services.EMail.TrySendMessageAsync(user, "E-Mail change", "Administration", $@"You are trying to change profile e-mail.
Current e-mail: {user.Email}
New e-mail: {NewEMail}
Follow this link to finish the operation: {confirmationLink}");

                if (isSent)
                {
                    LayoutModel.AddMessage($"Conformation link has been send to {NewEMail}");

                    return Redirect(Services.History.GetLastURL());
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
