using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Pages.Account;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Authorize()]
    public class AccountController : Controller
    {
        readonly BlogContext _db;
        readonly UserManager<User> _userManager;
        readonly SignInManager<User> _signInManager;
        readonly EMailConfirmationService _eMailConfirmationService;

        public AccountController(BlogContext db, UserManager<User> userManager, SignInManager<User> signInManager, EMailConfirmationService eMailConfirmationService)
        {
            _db = db;
            _userManager = userManager;
            _signInManager = signInManager;
            _eMailConfirmationService = eMailConfirmationService;
        }

        [HttpGet()]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToPage("/Index");
        }

        [HttpGet()]
        public async Task<IActionResult> ConfirmEMailByTokenAsync([Required]string confirmationToken)
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var expectedToken = _eMailConfirmationService.GetConfirmationToken(user);
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
                throw new Exception("Bad E-Mail confirmatin token");
            }
        }
    }
}