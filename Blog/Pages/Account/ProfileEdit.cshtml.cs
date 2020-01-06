using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Utilities;
using Utilities.Extensions;

namespace Blog.Pages.Account
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class ProfileEditModel : PageModelBase
    {
        [BindProperty]
        [MaxLength(5000)]
        public string About { get; set; }
        [BindProperty]
        [DataType(DataType.Upload)]
        [ImageFile(1 * 1024 * 1024)]
        public IFormFile ProfileImage { get; set; }
        [BindProperty]
        public string EditUserId { get; set; }

        public ProfileEditModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {
            
        }

        public async Task<IActionResult> OnGetAsync([Required]string id)
        {
            EditUserId = id;
            var currentUser = await S.UserManager.GetUserAsync(User);
            var editUser = await S.UserManager.FindByIdAsync(EditUserId);
            if (editUser != null &&
               (currentUser.Id == EditUserId || User.IsInOneOfTheRoles(Roles.GetAllNotLess(Roles.MODERATOR))))
            {
                About = editUser.Profile.About;

                return Page();
            }
            else
            {
                return RedirectToPage("/Index");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var editingUser = await getEditingUserIfAuthorizedAsync(EditUserId);
                if (ProfileImage != null)
                {
                    editingUser.Profile.Image = await S.Storage.SaveProfileImageAsync(ProfileImage, editingUser);
                }
                editingUser.Profile.About = About;
                currentUser.Actions.Add(new UserAction(ActionType.PROFILE_EDITED, editingUser.Profile));
                if (!await S.Permissions.CanEditProfileWithoutCheckAsync(editingUser))
                {
                    currentUser.ModeratorsInChargeGroup.AddEntityToCheck(editingUser.Profile, CheckReason.CHECK_REQUIRED);
                }
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Account/Profile", new { id = editingUser.Id });
            }
            else
            {
                return Page();
            }

            async Task<User> getEditingUserIfAuthorizedAsync(string userId)
            {
                var currentUser = await S.UserManager.GetUserAsync(User);
                var isEditAuthorized = currentUser?.Id == userId || User.IsInOneOfTheRoles(Roles.GetAllNotLess(Roles.MODERATOR));
                var editUser = await S.UserManager.FindByIdAsync(userId);

                return isEditAuthorized ? editUser : null;
            }
        }
    }
}