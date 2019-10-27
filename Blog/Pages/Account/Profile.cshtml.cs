using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages.Account
{
    public class ProfileModel : ExtendedPageModel
    {
        public User UserModel { get; private set; }
        public string Role { get; private set; }
        /// <summary>
        /// Page is shown to the current logged in user
        /// </summary>
        public bool IsCurrentUser { get; private set; }

        public ProfileModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var currentUser = await UserManager.GetUserAsync(HttpContext.User);
            if (id == null)
            {
                if (currentUser == null)
                {
                    throw new ArgumentOutOfRangeException("Can't determine target user");
                }
                else
                {
                    UserModel = currentUser;
                }
            }
            else
            {
                UserModel = await UserManager.FindByIdAsync(id);
            }

            IsCurrentUser = currentUser?.Id == UserModel.Id;
            UserModel.Posts = await DB.Posts.ToListAsync();
            Role = (await UserManager.GetRolesAsync(UserModel)).Single();
            UserModel.Profile.ViewStatistic.UpdateStatistic(currentUser);

            await DB.SaveChangesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostBanAsync(string id)
        {
            var currentUser = await UserManager.GetUserAsync(HttpContext.User);
            if (id == null)
            {
                if (currentUser == null)
                {
                    throw new ArgumentOutOfRangeException("Can't determine target user");
                }
                else
                {
                    UserModel = currentUser;
                }
            }
            else
            {
                UserModel = await UserManager.FindByIdAsync(id);
            }

            IsCurrentUser = currentUser?.Id == UserModel.Id;
            UserModel.Posts = await DB.Posts
                .IncludeAuthor()
                .Where(p => p.Author == UserModel)
                .ToListAsync();
            Role = (await UserManager.GetRolesAsync(UserModel)).Single();
            return Page();
        }
    }
}