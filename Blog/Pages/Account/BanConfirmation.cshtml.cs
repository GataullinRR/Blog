using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    public class BanConfirmationModel : PageModelBase
    {
        [BindProperty()]
        public string UserId { get; set; }
        [BindProperty, DataType(DataType.Date), Required]
        public DateTime BannedTill { get; set; } = DateTime.UtcNow;
        [BindProperty, DataType(DataType.MultilineText), MinLength(10), Required]
        public string Reason { get; set; }

        public BanConfirmationModel(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        public async Task OnGetAsync([Required]string id)
        {
            UserId = id;
            var targetUser = await S.UserManager.FindByIdAsync(id);
            await S.Permissions.ValidateBanUserAsync(targetUser);
        }

        public async Task<IActionResult> OnPostBanAsync()
        {
            if (ModelState.IsValid)
            {
                if (BannedTill.ToUniversalTime() <= DateTime.UtcNow)
                {
                    ModelState.AddModelError("", $"Selected date is less than current");

                    return Page();
                }
                else
                {
                    var targetUser = await S.UserManager.FindByIdAsync(UserId);
                    await S.Permissions.ValidateBanUserAsync(targetUser);
                    await S.Banning.BanAsync(targetUser, BannedTill, Reason);
                    LayoutModel.AddMessage($"User \"{targetUser.UserName}\" has been banned");

                    return Redirect("/Index");
                }
            }
            else
            {
                return Page();
            }
        }
    }
}