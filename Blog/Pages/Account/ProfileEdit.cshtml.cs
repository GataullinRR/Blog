using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ASPCoreUtilities;
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
    public class ProfileEditModel : PageModel
    {
        readonly BlogContext _db;
        readonly SignInManager<User> _signInManager;
        readonly UserManager<User> _userManager;

        [BindProperty]
        [MaxLength(5000)]
        public string About { get; set; }
        [BindProperty]
        [DataType(DataType.Upload)]
        [ImageFile(1 * 1024 * 1024)]
        public IFormFile ProfileImage { get; set; }
        [BindProperty]
        public string EditUserId { get; set; }


        public ProfileEditModel(BlogContext db, SignInManager<User> signInManager, UserManager<User> userManager)
        {
            _db = db;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            EditUserId = id;
            var user = await _userManager.GetUserAsync(User);
            if (user.Id == id || User.IsInOneOfTheRoles(Roles.ADMIN))
            {
                About = user.About;

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
                var user = await getUserIfAuthorizedAsync(EditUserId);

                if (ProfileImage != null)
                {
                    var serverLocalPath = Path.Combine("images", "users", user.Id.ToString(), $"{Guid.NewGuid()}.{Path.GetExtension(ProfileImage.FileName)}");
                    var serverPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", serverLocalPath);
                    using (var toServerStream = IOUtils.CreateFile(serverPath))
                    using (var fromClientStream = ProfileImage.OpenReadStream())
                    {
                        await toServerStream.WriteAsync(await fromClientStream.ReadToEndAsync());
                    }

                    user.ProfileImage = serverLocalPath;
                }
                user.About = About;
                await _db.SaveChangesAsync();

                return RedirectToPage("/Account/Profile", new { id = user.Id });
            }
            else
            {
                return Page();
            }

            async Task<User> getUserIfAuthorizedAsync(string userId)
            {
                var user = await _userManager.GetUserAsync(User);
                var isAuthorized = user.Id == userId || User.IsInOneOfTheRoles(Roles.ADMIN);

                return isAuthorized ? user : null;
            }
        }
    }
}