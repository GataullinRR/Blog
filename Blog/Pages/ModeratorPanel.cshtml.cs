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

namespace Blog.Pages
{
    public class ModeratorPanelModel : PageModelBase
    {
        public ModeratorPannel Panel { get; private set; }
        public IEnumerable<IEntityToCheck> Entities { get; private set; }
        public User Owner { get; private set; }
        public IEnumerable<User> TargetUsers { get; private set; }

        [Required, BindProperty, Range(0, 100)]
        public int NumOfEntitiesToAssign { get; set; }
        public bool ReadOnlyAccess { get; private set; }
        [BindProperty]
        public string TargetModeratorId { get; set; }

        public ModeratorPanelModel(ServicesProvider services) : base(services)
        {

        }

        public async Task OnGet(string id)
        {
            TargetModeratorId = id;
            var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
            Owner = await S.Utilities.FindUserByIdOrGetCurrentOrThrowAsync(id);
            await S.Permissions.ValidateAccessModeratorsPanelAsync(Owner);
            ReadOnlyAccess = Owner != currentUser;

            Panel = Owner.ModeratorPanel;
            Entities = Panel.EntitiesToCheck.Where(e => !e.IsResolved).OrderBy(e => e.AddTime);
            TargetUsers = S.Db.Users.Where(u => u.ModeratorsInCharge.Contains(Owner));
        }

        public async Task<IActionResult> OnPostAssign()
        {
            if (ModelState.IsValid)
            {
                await OnGet(TargetModeratorId);
                var entities = Entities.Where(e => !e.IsResolved).Take(NumOfEntitiesToAssign).ToArray();
                foreach (var entity in entities)
                {
                    entity.AssignedModerator = Owner;
                    entity.AssignationTime = DateTime.UtcNow;
                }
                await S.Db.SaveChangesAsync();

                LayoutModel.AddMessage($"{entities.Length} new entities were assigned to you");

                return Page();
            }
            else
            {
                return Page();
            }
        }

        public async Task<IActionResult> OnGetCheck([Required]int id)
        {
            if (ModelState.IsValid)
            {
                await OnGet(TargetModeratorId);
                var entity = Panel.EntitiesToCheck.First(e => e.Id == id);
                entity.ResolvingTime = DateTime.UtcNow;
                Owner.Actions.Add(new UserAction(ActionType.REPORT_CHECKED, entity));

                await S.Db.SaveChangesAsync();

                return Page();
            }
            else
            {
                return Page();
            }
        }
    }
}