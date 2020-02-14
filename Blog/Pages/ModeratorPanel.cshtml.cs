using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class ModeratorPanelModel : PageModelBase
    {
        public ModeratorsGroup Group { get; private set; }
        public IEnumerable<IEntityToCheck> Entities { get; private set; }
        public User Owner { get; private set; }
        public IEnumerable<User> TargetUsers { get; private set; }
        [BindProperty]
        public EntitiesAssignModel AssignModel { get; private set; }

        public bool ReadOnlyAccess { get; private set; }
        public string TargetModeratorId { get; set; }

        public ModeratorPanelModel(ServiceLocator services) : base(services)
        {

        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            TargetModeratorId = id ?? currentUser.Id;
            Owner = await S.Db.Users.AsNoTracking()
                .Include(u => u.ModeratorsGroup)
                .FirstOrDefaultAsync(u => u.Id == TargetModeratorId);
            await S.Permissions.ValidateAccessModeratorsPanelAsync(Owner);
            ReadOnlyAccess = Owner.Id != currentUser.Id;

            Group = await S.Db.ModeratorsGroups.AsNoTracking()
                .Include(g => g.TargetUsers)
                .Include(g => g.Moderators)
                .IncludeEntitiesToCheck()
                .FirstOrDefaultAsync(g => g.Id == Owner.ModeratorsGroup.Id);
            Entities = Group.EntitiesToCheck.OrderBy(e => e.AddTime);
            TargetUsers = Owner.ModeratorsGroup.TargetUsers;

            AssignModel = new EntitiesAssignModel()
            {
                ModeratorId = Owner.Id,
                NumOfEntitiesToAssign = 0
            };

            return Page();
        }
    }
}