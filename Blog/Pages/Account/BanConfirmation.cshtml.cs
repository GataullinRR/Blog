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

        public BanConfirmationModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task OnGetAsync(string id)
        {
            UserId = id;
            var targetUser = await Services.UserManager.FindByIdAsync(id);
            await Services.Permissions.ValidateBanUserAsync(targetUser);
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
                    var targetUser = await Services.UserManager.FindByIdAsync(UserId);
                    await Services.Permissions.ValidateBanUserAsync(targetUser);

                    targetUser.Status.State = DBModels.ProfileState.BANNED;
                    targetUser.Status.BannedTill = BannedTill.ToUniversalTime();
                    targetUser.Status.StateReason = Reason;
                    targetUser.Actions.Add(new UserAction(ActionType.BAN, targetUser.Id));
                    await Services.Db.SaveChangesAsync();

                    await Services.EMail.TrySendMessageAsync(targetUser, "Administration", "Profile has been banned", $@"Profile name: {targetUser.UserName}
Reason: {Reason}
Ban will expire at {BannedTill.ToString("dd.MM.yyyy")}");

                    LayoutModel.AddMessage($"User \"{targetUser.UserName}\" has been banned till {BannedTill.ToString("dd.MM.yyyy")}");

                    return RedirectToPage("/Index");
                }
            }
            else
            {
                return Page();
            }
        }
    }
}