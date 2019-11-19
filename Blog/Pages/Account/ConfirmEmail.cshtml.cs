using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    public class ConfirmEMailModel : PageModelBase
    {
        public User UserModel { get; private set; }
        public bool IsConfirmationLinkSent { get; private set; }

        public ConfirmEMailModel(ServicesProvider services) : base(services)
        {

        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await S.Utilities.GetCurrentUserModelOrThrowAsync();

            if (UserModel.EmailConfirmed)
            {
                return Page();
            }
            else
            {
                var confirmationUrl = await S.ConfirmationLinks.GetEMailConfirmationLinkAsync(UserModel);
                var message = $@"Hi {UserModel.UserName}!

Please follow this link to complete the registration: {confirmationUrl}";
                IsConfirmationLinkSent = await S.EMail.TrySendMessageAsync(UserModel, "Registration", "EMail confirmation", message);

                return Page();
            }
        }
    }
}