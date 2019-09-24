using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Pages.Account;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        readonly AutentificationService _autentification;

        public AccountController(AutentificationService autentification)
        {
            _autentification = autentification;
        }

        [HttpGet()]
        public async Task<IActionResult> Logout()
        {
            await _autentification.LogoutAsync(HttpContext);

            return RedirectToPage("/Index");
        }

        [HttpPost()]
        public async Task<IActionResult> AddCommentary(int postId, string commentBody)
        {
            return RedirectToPage("/Index");
        }
    }
}