using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Controllers;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages
{
    public class BlogControlPanelModel : PageModelBase
    {
        public IEnumerable<User> Moderators { get; private set; }

        public BlogControlPanelModel(ServicesProvider services) : base(services)
        {
        }
        
        public async Task OnGetAsync()
        {
            await Services.Permissions.ValidateAccessBlogControlPanelAsync();
            Moderators = await Services.UserManager.GetUsersInRoleAsync(Roles.MODERATOR);
        }

        public async Task<IActionResult> OnGetModeratorInvitationLinkAsync()
        {
            await Services.Permissions.ValidateAccessBlogControlPanelAsync();

            var provider = await GetCurrentUserModelOrThrowAsync();
            var link = await Services.ActivationLinks.GenerateRegisterAsModeratorLink(provider);
            LayoutModel.AddMessage($"Moderator activation link: {link}");

            return Page();
        }

        public async Task<IActionResult> OnGetOwnerInvitationLinkAsync()
        {
            await Services.Permissions.ValidateAccessBlogControlPanelAsync();

            var provider = await GetCurrentUserModelOrThrowAsync();
            var link = await Services.ActivationLinks.GenerateRegisterAsOwnerLink(provider);
            LayoutModel.AddMessage($"Owner activation link: {link}");

            return Page();
        }
    }
}