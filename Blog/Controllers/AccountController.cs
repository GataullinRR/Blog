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
    public class AccountController : ControllerBase
    {
        public AccountController(ServicesProvider serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet()]
        public async Task CheckIfAuthentificated()
        {
            await GetCurrentUserModelOrThrowAsync();
        }

        [HttpGet()]
        public async Task<IActionResult> Logout()
        {
            await Services.Permissions.ValidateLogoutAsync();

            var currenUser = await Services.UserManager.GetUserAsync(User);
            await Services.SignInManager.SignOutAsync();
            if (currenUser != null)
            {
                currenUser.Actions.Add(new DBModels.UserAction(ActionType.SIGNED_OUT, null));
                await Services.Db.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }

        [HttpGet()]
        public async Task<IActionResult> UnbanAsync([Required]string userId)
        {
            if (ModelState.IsValid)
            {
                var targetUser = await Services.UserManager.FindByIdAsync(userId);
                await Services.Permissions.ValidateUnbanUserAsync(targetUser);

                targetUser.Status.State = targetUser.EmailConfirmed
                    ? ProfileState.ACTIVE
                    : ProfileState.RESTRICTED;
                targetUser.Status.StateReason = null;
                targetUser.Status.BannedTill = null;
                targetUser.Actions.Add(new UserAction(ActionType.UNBAN, targetUser));
                await Services.Db.SaveChangesAsync();

                LayoutModel.AddMessage($"User \"{targetUser.UserName}\" has been unbanned");

                return RedirectToPage("/Account/Profile", new { id = userId });
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task<IActionResult> ConfirmAsync([Required]string userId, [Required]string token, [Required]AccountOperation operation, string arguments)
        {
            if (ModelState.IsValid)
            {
                var user = await Services.UserManager.FindByIdAsync(userId);
                var isVerified = await Services.Confirmation.VerifyTokenAsync(user, operation, token);
                if (isVerified)
                {
                    switch (operation)
                    {
                        case AccountOperation.EMAIL_CONFIRMATION:
                            return await confirmEmail(user);
                        case AccountOperation.PASSWORD_RESET:
                            return await generateAndSendNewPasswordAsync(user);
                        case AccountOperation.EMAIL_CHANGE:
                            return await changeEMail(user, arguments);

                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    return reportError("Bad confirmation token");
                }
            }
            else
            {
                return reportError("Bad arguments");
            }
        }
        async Task<IActionResult> changeEMail(User user, string arguments)
        {
            if (arguments == null)
            {
                return reportError("Bad arguments");
            }
            else if (await GetCurrentUserModelOrThrowAsync() != user)
            {
                return reportError("You should log in in order this link to work");
            }
            else
            {
                var newEmail = arguments;
                user.Email = newEmail;
                user.Actions.Add(new DBModels.UserAction(DBModels.ActionType.EMAIL_CHANGED, user));
                await Services.Db.SaveChangesAsync();

                LayoutModel.AddMessage($"Email has been changed to {newEmail}");

                return RedirectToPage("/Account/Profile");
            }
        }
        async Task<IActionResult> confirmEmail(User user)
        {
            if (user.EmailConfirmed)
            {
                return RedirectToPage("/Index");
            }
            else if (await GetCurrentUserModelOrThrowAsync() != user)
            {
                return reportError("You should log in in order this link to work");
            }
            else
            {
                user.EmailConfirmed = true;
                await Services.UserManager.AddToRoleAsync(user, Roles.USER);
                user.Actions.Add(new UserAction(ActionType.EMAIL_CONFIRMED, user));
                await Services.Db.SaveChangesAsync();

                LayoutModel.AddMessage("Email has been confirmed!");

                return RedirectToPage("/Index");
            }
        }
        async Task<IActionResult> generateAndSendNewPasswordAsync(User user)
        {
            var newPassword = 3.Times(_ => Global.Random.NextENWord().Capitalize()).Aggregate("");
            var isSent = await Services.EMail.TrySendMessageAsync(user, "Password reset", "New password", $@"Your new password is: {newPassword}
Please delete this message so that nobody can see it");
            if (isSent)
            {
                user.PasswordHash = Services.UserManager.PasswordHasher.HashPassword(user, newPassword);
                var result = await Services.UserManager.UpdateAsync(user);
                await Services.SignInManager.SignOutAsync();
                user.Actions.Add(new UserAction(ActionType.PASSWORD_RESET, null));
                await Services.Db.SaveChangesAsync();

                LayoutModel.AddMessage("New password has been sent to your E-Mail");

                return RedirectToPage("/Account/Login");
            }
            else
            {
                return reportError("Could not send the E-Mail");
            }
        }
        IActionResult reportError(string message)
        {
            LayoutModel.AddMessage(message);

            return RedirectToPage("/Index");
        }
    }
}