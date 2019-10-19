using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
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
    public class ConfirmEMailModel : PageModel
    {
        readonly EMailService _eMailService;
        readonly ConfirmationTokenService _conformation;
        readonly UserManager<User>  _userManager;
        
        public User UserModel { get; private set; }
        public bool IsConfirmationLinkSent { get; private set; }

        public ConfirmEMailModel(EMailService eMailService, ConfirmationTokenService eMailConformation, UserManager<User> userManager)
        {
            _eMailService = eMailService;
            _conformation = eMailConformation;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await _userManager.GetUserAsync(HttpContext.User);

            if (UserModel.EmailConfirmed)
            {
                return Page();
            }
            else
            {
                var confirmationUrl = Url.Action(nameof(AccountController.ConfirmEMailByTokenAsync), nameof(AccountController).SkipLast(10).Aggregate(), new { confirmationToken = _conformation.GetToken(UserModel, AccountOperation.EMAIL_CONFIRMATION) }, Url.ActionContext.HttpContext.Request.Scheme);
                var message = $@"Hi {UserModel.UserName}!

Please follow this link to complete the registration: {confirmationUrl}";
                IsConfirmationLinkSent = await _eMailService.TrySendMessageAsync(UserModel, "Registration", "EMail confirmation", message);

                return Page();
            }
        }
    }
}