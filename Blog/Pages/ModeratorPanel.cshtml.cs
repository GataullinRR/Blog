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
        public User Moderator { get; private set; }
        public IEnumerable<User> TargetUsers { get; private set; }

        [Required, BindProperty, Range(0, 100)]
        public int NumOfEntitiesToAssign { get; set; }

        public ModeratorPanelModel(ServicesProvider services) : base(services)
        {
            PersistLayoutModel = true;
        }

        public async Task OnGet()
        {
            await Services.Permissions.ValidateAccessModeratorsPanelAsync();

            Moderator = await GetCurrentUserModelOrThrowAsync();
            Panel = Moderator.ModeratorPanel;
            Entities = Panel.EntitiesToCheck.Where(e => !e.IsResolved).OrderBy(e => e.AddTime);
            TargetUsers = Services.Db.Users.Where(u => u.ModeratorsInCharge.Contains(Moderator));
        }

        public async Task<IActionResult> OnPostAssign()
        {
            if (ModelState.IsValid)
            {
                await OnGet();
                var entities = Entities.Where(e => !e.IsResolved).Take(NumOfEntitiesToAssign).ToArray();
                foreach (var entity in entities)
                {
                    entity.AssignedModerator = Moderator;
                    entity.AssignationTime = DateTime.UtcNow;
                }
                await Services.Db.SaveChangesAsync();

                LayoutModel.Messages.Add($"{entities.Length} new entities were assigned to you");

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
                await OnGet();
                Panel.EntitiesToCheck.First(e => e.Id == id).ResolvingTime = DateTime.UtcNow;
                await Services.Db.SaveChangesAsync();

                return Page();
            }
            else
            {
                return Page();
            }
        }
    }
}