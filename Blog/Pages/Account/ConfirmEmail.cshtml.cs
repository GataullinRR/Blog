using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    [Authorize]
    public class ConfirmEMailModel : PageModel
    {
        readonly AutentificationService _autentification;
        readonly EMailService _eMailService;
        readonly EMailConfirmationService _eMailConformation;

        public User UserModel { get; private set; }
        public bool IsConfirmationLinkSent { get; private set; }

        public ConfirmEMailModel(AutentificationService autentification, EMailService eMailService, EMailConfirmationService eMailConformation)
        {
            _autentification = autentification;
            _eMailService = eMailService;
            _eMailConformation = eMailConformation;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            UserModel = await _autentification.GetCurrentUserAsync(HttpContext);

            if (UserModel.IsEmailConfirmed)
            {
                return Page();
            }
            else
            {
                var confirmationUrl = Url.Action(nameof(AccountController.ConfirmEMailByTokenAsync), nameof(AccountController).SkipLast(10).Aggregate(), new { confirmationToken = _eMailConformation.GetConfirmationToken(UserModel) }, Url.ActionContext.HttpContext.Request.Scheme);
                var message = $@"Hi {UserModel.Nickname}!

Please follow this link to complete the registration: {confirmationUrl}";
                IsConfirmationLinkSent = await _eMailService.TrySendMessageAsync(UserModel, "Registration", "EMail confirmation", message);

                return Page();
            }
        }
    }
}