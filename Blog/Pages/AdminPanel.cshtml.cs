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
    public class AdminPanelModel : PageModelBase
    {
        public IEnumerable<User> Moderators { get; private set; }
        public IQueryable<ModeratorsGroup> ModeratorsGroups { get; private set; }

        public AdminPanelModel(ServicesProvider services) : base(services)
        {

        }
        
        public async Task OnGetAsync()
        {
            await S.Permissions.ValidateAccessBlogControlPanelAsync();

            Moderators = await S.UserManager.GetUsersInRoleAsync(Roles.MODERATOR);
            ModeratorsGroups = S.Db.ModeratorsGroups;
        }
    }
}