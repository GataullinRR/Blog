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
        public AccountController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            PersistLayoutModel = true;
        }

        [HttpGet(), Authorize()]
        public async Task<IActionResult> Logout()
        {
            var currenUser = await UserManager.GetUserAsync(User);
            await SignInManager.SignOutAsync();
            currenUser.Actions.Add(new DBModels.UserAction(ActionType.SIGNED_OUT, null));
            await DB.SaveChangesAsync();

            return RedirectToPage("/Index");
        }

        [HttpGet(), Authorize()]
        public async Task<IActionResult> UnbanAsync([Required]string userId)
        {
            if (ModelState.IsValid)
            {
                var targetUser = await UserManager.FindByIdAsync(userId);
                await Permissions.ValidateUnbanUserAsync(targetUser);

                targetUser.Status.State = targetUser.EmailConfirmed
                    ? ProfileState.ACTIVE
                    : ProfileState.RESTRICTED;
                targetUser.Status.StateReason = null;
                targetUser.Status.BannedTill = null;
                targetUser.Actions.Add(new DBModels.UserAction(ActionType.UNBAN, DateTime.UtcNow, targetUser.Id));
                await DB.SaveChangesAsync();

                LayoutModel.Messages.Add($"User \"{targetUser.UserName}\" has been unbanned");

                return RedirectToPage("/Account/Profile", new { id = userId });
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet(), Authorize()]
        public async Task<IActionResult> ConfirmEMailByTokenAsync([Required]string confirmationToken)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.GetUserAsync(HttpContext.User);
                var expectedToken = ConfirmationTokens.GetToken(user, AccountOperation.EMAIL_CONFIRMATION);
                if (confirmationToken == expectedToken)
                {
                    if (user.EmailConfirmed)
                    {
                        return RedirectToPage("/Index");
                    }
                    else
                    {
                        user.EmailConfirmed = true;
                        await UserManager.AddToRoleAsync(user, Roles.USER);
                        user.Actions.Add(new DBModels.UserAction(ActionType.EMAIL_CONFIRMATION, DateTime.UtcNow, null));
                        await DB.SaveChangesAsync();

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
                var user = await UserManager.FindByIdAsync(userId);
#warning add something similar
                //_permissions.ValidateResetPassword(User, user);

                var expectedToken = ConfirmationTokens.GetToken(user, AccountOperation.PASSWORD_RESET);
                if (confirmationToken == expectedToken)
                {
                    var newPassword = 3.Times(_ => Global.Random.NextENWord().Capitalize()).Aggregate("");
                    var isSent = await EMail.TrySendMessageAsync(user, "Password reset", "New password", $@"Your new password is: {newPassword}
Please delete this message so that nobody can see it");
                    if (isSent)
                    {
                        user.PasswordHash = UserManager.PasswordHasher.HashPassword(user, newPassword);
                        var result = await UserManager.UpdateAsync(user);
                        await SignInManager.SignOutAsync();
                        user.Actions.Add(new DBModels.UserAction(ActionType.PASSWORD_RESET, DateTime.UtcNow, null));
                        await DB.SaveChangesAsync();

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