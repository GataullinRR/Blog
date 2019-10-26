using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    public class BanConfirmationModel : ExtendedPageModel
    {
        [BindProperty()]
        public string UserId { get; set; }
        [BindProperty, DataType(DataType.Date), Required]
        public DateTime BannedTill { get; set; } = DateTime.UtcNow;
        [BindProperty, DataType(DataType.MultilineText), MinLength(10), Required]
        public string Reason { get; set; }

        public BanConfirmationModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            PersistLayoutModel = true;
        }

        public async Task OnGetAsync(string id)
        {
            UserId = id;
            var targetUser = await UserManager.FindByIdAsync(id);
            await Permissions.ValidateBanUserAsync(targetUser);
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
                    var targetUser = await UserManager.FindByIdAsync(UserId);
                    await Permissions.ValidateBanUserAsync(targetUser);

                    targetUser.Status.State = DBModels.ProfileState.BANNED;
                    targetUser.Status.BannedTill = BannedTill.ToUniversalTime();
                    targetUser.Status.StateReason = Reason;
                    await DB.SaveChangesAsync();

                    await EMail.TrySendMessageAsync(targetUser, "Administration", "Profile has been banned", $@"Profile name: {targetUser.UserName}
Reason: {Reason}
Ban will expire at {BannedTill.ToString("dd.MM.yyyy")}");

                    LayoutModel.Messages.Add($"User \"{targetUser.UserName}\" has been banned till {BannedTill.ToString("dd.MM.yyyy")}");

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