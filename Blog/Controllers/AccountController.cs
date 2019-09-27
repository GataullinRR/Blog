using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Pages.Account;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        readonly BlogContext _db;
        readonly AutentificationService _autentification;
        readonly EMailConfirmationService _eMailConfirmationService;

        public AccountController(BlogContext db, AutentificationService autentification, EMailConfirmationService eMailConfirmationService)
        {
            _db = db;
            _autentification = autentification;
            _eMailConfirmationService = eMailConfirmationService;
        }

        [HttpGet()]
        public async Task<IActionResult> Logout()
        {
            await _autentification.LogoutAsync(HttpContext);

            return RedirectToPage("/Index");
        }

        [HttpGet()]
        public async Task<IActionResult> ConfirmEMailByTokenAsync([Required]string confirmationToken)
        {
            var user = await _autentification.GetCurrentUserAsync(HttpContext);
            var expectedToken = _eMailConfirmationService.GetConfirmationToken(user);
            if (confirmationToken == expectedToken)
            {
                if (user.IsEmailConfirmed)
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    user.IsEmailConfirmed = true;
                    user.Role = Role.USER;
                    await _db.SaveChangesAsync();

                    return RedirectToPage("/Account/ConfirmEMail");
                }
            }
            else
            {
                throw new Exception("Bad E-Mail confirmatin token");
            }
        }
    }
}