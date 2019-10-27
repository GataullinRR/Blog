using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities;
using Blog.Models;
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
    public class ProfileEditModel : ExtendedPageModel
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

        public ProfileEditModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            
        }

        public async Task<IActionResult> OnGetAsync([Required]string id)
        {
            EditUserId = id;
            var currentUser = await UserManager.GetUserAsync(User);
            var editUser = await UserManager.FindByIdAsync(EditUserId);
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
                var currentUser = await GetCurrentUserModelOrThrowAsync();
                var editingUser = await getEditingUserIfAuthorizedAsync(EditUserId);
                if (ProfileImage != null)
                {
                    var serverLocalPath = Path.Combine("images", "users", editingUser.Id.ToString(), $"{Guid.NewGuid()}.{Path.GetExtension(ProfileImage.FileName)}");
                    var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverLocalPath);
                    using (var toServerStream = IOUtils.CreateFile(serverPath))
                    using (var fromClientStream = ProfileImage.OpenReadStream())
                    {
                        await toServerStream.WriteAsync(await fromClientStream.ReadToEndAsync());
                    }

                    editingUser.Profile.Image = serverLocalPath;
                }
                editingUser.Profile.About = About;
                currentUser.Actions.Add(new UserAction(ActionType.PROFILE_EDIT, editingUser.Id));
                await DB.SaveChangesAsync();

                return RedirectToPage("/Account/Profile", new { id = editingUser.Id });
            }
            else
            {
                return Page();
            }

            async Task<User> getEditingUserIfAuthorizedAsync(string userId)
            {
                var currentUser = await UserManager.GetUserAsync(User);
                var isEditAuthorized = currentUser?.Id == userId || User.IsInOneOfTheRoles(Roles.GetAllNotLess(Roles.MODERATOR));
                var editUser = await UserManager.FindByIdAsync(userId);

                return isEditAuthorized ? editUser : null;
            }
        }
    }
}