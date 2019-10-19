using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Pages.Account;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Utilities;
using Utilities.Extensions;

namespace Blog.Controllers
{
    
    public class AccountController : ExtendedController
    {
        readonly BlogContext _db;
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        readonly ConfirmationTokenService _confirmationService;
        readonly EMailService _email;
        readonly PermissionsService _permissions;

        public AccountController(BlogContext db, UserManager<User> userManager, SignInManager<User> signInManager, ConfirmationTokenService confirmationService, EMailService email, PermissionsService permissions)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
            _confirmationService = confirmationService ?? throw new ArgumentNullException(nameof(confirmationService));
            _email = email ?? throw new ArgumentNullException(nameof(email));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));

            PersistLayoutModel = true;
        }

        [HttpGet(), Authorize()]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }

        [HttpGet(), Authorize()]
        public async Task<IActionResult> ConfirmEMailByTokenAsync([Required]string confirmationToken)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(HttpContext.User);
                var expectedToken = _confirmationService.GetToken(user, AccountOperation.EMAIL_CONFIRMATION);
                if (confirmationToken == expectedToken)
                {
                    if (user.EmailConfirmed)
                    {
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        user.EmailConfirmed = true;
                        await _userManager.RemoveFromRoleAsync(user, Roles.UNCONFIRMED);
                        await _userManager.AddToRoleAsync(user, Roles.USER);
                        await _db.SaveChangesAsync();

                        return RedirectToPage("/Account/ConfirmEMail"); // Will show greeting
                    }
                }
                else
                {
                    throw new Exception("Bad E-Mail confirmation token");
                }
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task<IActionResult> ConfirmPasswordResetAsync([Required]string userId, [Required]string confirmationToken)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);
#warning add something similar
                //_permissions.ValidateResetPassword(User, user);

                var expectedToken = _confirmationService.GetToken(user, AccountOperation.PASSWORD_RESET);
                if (confirmationToken == expectedToken)
                {
                    var newPassword = 3.Times(_ => Global.Random.NextENWord().Capitalize()).Aggregate("");
                    var isSent = await _email.TrySendMessageAsync(user, "Password reset", "New password", $@"Your new password is: {newPassword}
Please delete this message so that nobody can see it");
                    if (isSent)
                    {
                        user.PasswordHash = _userManager.PasswordHasher.HashPassword(user, newPassword);
                        var result = await _userManager.UpdateAsync(user);
                        await _signInManager.SignOutAsync();

                        LayoutModel.Messages.Add("New password has been sent to your E-Mail");

                        return RedirectToPage("/Account/Login");
                    }
                    else
                    {
                        return reportError("Could not send an E-Mail");
                    }
                }
                else
                {
                    return reportError("Bad confirmation token");
                }
            }
            else
            {
                throw new Exception();
            }
        }

        IActionResult reportError(string message)
        {
            LayoutModel.Messages.Add(message);

            return RedirectToPage("/Index");
        }
    }
}