using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blog.Misc;
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
        public AccountController(ServiceLocator serviceProvider) : base(serviceProvider)
        {
        }

        [HttpGet()]
        public async Task CheckIfAuthentificated()
        {
            await S.Utilities.GetCurrentUserModelOrThrowAsync();
        }

        [HttpGet()]
        public async Task<IActionResult> Logout()
        {
            await S.Permissions.ValidateLogoutAsync();

            var currenUser = await S.UserManager.GetUserAsync(User);
            await S.SignInManager.SignOutAsync();
            if (currenUser != null)
            {
                await S.Repository.AddUserActionAsync(currenUser, new UserAction(ActionType.SIGNED_OUT, currenUser));
                await S.Db.SaveChangesAsync();
            }

            return RedirectToPage("/Index");
        }

        [HttpGet()]
        public async Task<IActionResult> UnbanAsync([Required]string userId)
        {
            if (ModelState.IsValid)
            {
                var targetUser = await S.UserManager.FindByIdAsync(userId);
                await S.Permissions.ValidateUnbanUserAsync(targetUser);
                await S.Banning.UnbanAsync(targetUser);

                LayoutModel.AddMessage($"User \"{targetUser.UserName}\" has been unbanned");

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task BanForeverAsync([Required]string userId, string reason)
        {
            if (ModelState.IsValid)
            {
                var targetUser = await S.UserManager.FindByIdAsync(userId);
                await S.Permissions.ValidateBanUserAsync(targetUser);
                await S.Banning.BanForeverAsync(targetUser, reason);
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task<IActionResult> ApplyActivationLinkAsync([Required]string token)
        {
            if (ModelState.IsValid)
            {
                var tokenData = await S.ActivationLinks.ParseAsync(token);
                if (tokenData.Validity != TokenValidity.VALID)
                {
                    return reportError(tokenData.Validity.GetEnumValueDescription());
                }

                var role = "";
                switch (tokenData.Data.Action)
                {
                    case ActivationLinkAction.REGISTER_AS_OWNER:
                        role = Roles.ADMINISTRATOR;
                        break;
                    case ActivationLinkAction.REGISTER_AS_MODERATOR:
                        role = Roles.MODERATOR;
                        break;

                    default:
                        throw new NotSupportedException();
                }

                if (User.Identity.IsAuthenticated)
                {
                    await Logout();
                }
                S.MutatorsManager.RegistrationRole = role;
                await S.ActivationLinks.MarkAsUsedOrExpiredAsync(token);
                return RedirectToPage("/Account/Register");
            }
            else
            {
                return reportError("Bad arguments");
            }
        }

        [HttpGet()]
        public async Task<IActionResult> ConfirmAsync([Required]string token)
        {
            if (ModelState.IsValid)
            {
                var tokenData = await S.ConfirmationLinks.ParseAsync(token);
                if (tokenData.Validity != TokenValidity.VALID)
                {
                    return reportError(tokenData.Validity.GetEnumValueDescription());
                }

                IActionResult view = null;
                switch (tokenData.Data.Operation)
                {
                    case AccountOperation.EMAIL_CONFIRMATION:
                        view = await confirmEmail(tokenData.Data.Target);
                        break;
                    case AccountOperation.PASSWORD_RESET:
                        view = await generateAndSendNewPasswordAsync(tokenData.Data.Target);
                        break;
                    case AccountOperation.EMAIL_CHANGE:
                        view = await changeEMail(tokenData.Data.Target, tokenData.Data.Argument);
                        break;

                    default:
                        throw new NotSupportedException();
                }

                await S.ConfirmationLinks.MarkAsUsedOrExpiredAsync(token);
                
                return view;
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
            else if (await S.Utilities.GetCurrentUserModelOrThrowAsync() != user)
            {
                return reportError("You should log in in order this link to work");
            }
            else
            {
                var newEmail = arguments;
                user.Email = newEmail;
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.EMAIL_CHANGED, user));
                await S.Db.SaveChangesAsync();

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
            else if (await S.Utilities.GetCurrentUserModelOrThrowAsync() != user)
            {
                return reportError("You should be signed in in order this link to work");
            }
            else
            {
                user.EmailConfirmed = true;
                user.Status.State = ProfileState.ACTIVE;
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.EMAIL_CONFIRMED, user));
                await S.Db.SaveChangesAsync();

                LayoutModel.AddMessage("Email has been confirmed!");

                return RedirectToPage("/Index");
            }
        }
        async Task<IActionResult> generateAndSendNewPasswordAsync(User user)
        {
            var newPassword = 3.Times(_ => Global.Random.NextENWord().Capitalize()).Aggregate("");
            var isSent = await S.EMail.TrySendMessageAsync(user, "Password reset", "New password", $@"Your new password is: {newPassword}
Please delete this message so that nobody can see it");
            if (isSent)
            {
                user.PasswordHash = S.UserManager.PasswordHasher.HashPassword(user, newPassword);
                var result = await S.UserManager.UpdateAsync(user);
                await S.SignInManager.SignOutAsync();
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.PASSWORD_RESET, user));
                await S.Db.SaveChangesAsync();

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