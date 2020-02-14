using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Attributes;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [Authorize]
    public class ModerationController : ControllerBase
    {
        public static readonly string MARK_COMMENTARY_URI = getURIToAction(nameof(ModerationController), nameof(MarkCommentaryAsResolvedAsync));
        public static readonly string MARK_POST_URI = getURIToAction(nameof(ModerationController), nameof(MarkPostAsResolvedAsync));
        public static readonly string MARK_PROFILE_URI = getURIToAction(nameof(ModerationController), nameof(MarkProfileAsResolvedAsync));

        public ModerationController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> MarkPostAsModeratedAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var post = await S.Db.Posts
                    .Include(p => p.ModerationInfo)
                    .FirstOrDefaultAsync(p => p.Id == id);
                await S.Moderation.MarkPostAsModeratedAsync(post);

                return RedirectToPage("/Post", new { id = id });
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost]
        public async Task<IActionResult> AssignEntitiesAsync(EntitiesAssignModel model)
        {
            if (ModelState.IsValid)
            {
                var Owner = await S.Db.Users
                    .IncludeEntitiesToCheckSlim()
                    .FirstOrDefaultAsync(u => u.Id == model.ModeratorId);
                await S.Permissions.ValidateAccessModeratorsPanelAsync(Owner);

                var entities = Owner.ModeratorsGroup.EntitiesToCheck
                    .Where(e => !e.IsResolved)
                    .Take(model.NumOfEntitiesToAssign)
                    .ToArray();
                foreach (var entity in entities)
                {
                    entity.AssignedModerator = Owner;
                    entity.AssignationTime = DateTime.UtcNow;
                }
                await S.Db.SaveChangesAsync();

                LayoutModel.AddMessage($"{entities.Length} new entities were assigned to you");

                return RedirectToPage("/ModeratorPanel", new { id = Owner.Id });
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet, AJAX]
        public async Task<IActionResult> MarkCommentaryAsResolvedAsync([Required]string moderatorId, [Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await markAsResolvedAsync(moderatorId, id, S.Db.CommentariesToCheck);
            }
            else
            {
                throw new Exception();
            }
        }
        [HttpGet, AJAX]
        public async Task<IActionResult> MarkPostAsResolvedAsync([Required]string moderatorId, [Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await markAsResolvedAsync(moderatorId, id, S.Db.PostsToCheck);
            }
            else
            {
                throw new Exception();
            }
        }
        [HttpGet, AJAX]
        public async Task<IActionResult> MarkProfileAsResolvedAsync([Required]string moderatorId, [Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await markAsResolvedAsync(moderatorId, id, S.Db.ProfilesToCheck);
            }
            else
            {
                throw new Exception();
            }
        }

        async Task<IActionResult> markAsResolvedAsync<T>(string moderatorId, int entityId, IQueryable<EntityToCheck<T>> entities)
        {
            var owner = await S.Db.Users.AsNoTracking()
                .FirstOrDefaultAsync(u => u.Id == moderatorId);
            await S.Permissions.ValidateMarkAsResolvedAsync(owner);
            var entity = await entities
                .Where(c => c.AssignedModerator.Id == owner.Id && c.ResolvingTime == null)
                .Include(e => e.Entity)
                .FirstOrDefaultAsync(e => e.Id == entityId);
            entity.ResolvingTime = DateTime.UtcNow;
            await S.Repository.AddUserActionAsync(owner, new UserAction(ActionType.ENTITY_RESOLVED, entity.Entity));
            await S.Db.SaveChangesAsync();

            return new EmptyResult();
        }
    }
}