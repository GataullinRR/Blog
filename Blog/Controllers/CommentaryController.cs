using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Blog.Controllers
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class CommentaryController : ControllerBase
    {
        public const int MAX_COMMENTARY_LENGTH = 500;
        public const int MIN_COMMENTARY_LENGTH = 10;
        public const int MIN_EDIT_REASON_LENGTH = 10;
        public const int MAX_EDIT_REASON_LENGTH = 100;

        public CommentaryController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryEditAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await S.Db.Commentaries.AsNoTracking()
                    .Include(c => c.Author)
                    .FirstOrDefaultAsync(c => c.Id == id);
                var model = new CommentaryEditinigModel()
                {
                    Author = commentary.Author.UserName,
                    AuthorId = commentary.Author.Id,
                    Body = commentary.Body,
                    CommentaryId = commentary.Id,
                };

                return PartialView(Partials.COMMENTARY_EDIT, model);
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost()]
        public async Task<IActionResult> UpdateCommentaryAsync([Required]CommentaryEditinigModel model)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.UserManager.GetUserAsync(User);
                var commentary = await S.Db.Commentaries.FirstAsync(c => c.Id == model.CommentaryId);
                await S.Permissions.ValidateEditCommentaryAsync(commentary);

                var user = await S.UserManager.GetUserAsync(User);
                commentary.Body = model.Body;
                commentary.Edits.Add(new CommentaryEdit(user, model.Reason, DateTime.UtcNow));
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.COMMENTARY_EDIT, commentary));
                await S.Db.SaveChangesAsync();
            }
            else
            {
                if (model.Reason == null)
                {
                    LayoutModel.AddMessage("Edit reason is required");
                }
                else if (model.Body == null)
                {
                    LayoutModel.AddMessage("Commentary body is required");
                }
                else
                {
                    throw new Exception();
                }
            }

            return Redirect(S.History.GetLastURL());
        }

        [HttpGet()]
        public async Task<IActionResult> DeleteCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var commentary = await S.Db.Commentaries.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateDeleteCommentaryAsync(commentary);
               
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.COMMENTARY_DELETE, commentary));
                commentary.IsDeleted = true;
                await S.Db.SaveChangesAsync();

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet()]
        public async Task<IActionResult> RestoreCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var user = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var commentary = await S.Db.Commentaries.FirstOrDefaultByIdAsync(id);
                await S.Permissions.ValidateRestoreCommentaryAsync(commentary);
               
                await S.Repository.AddUserActionAsync(user, new UserAction(ActionType.COMMENTARY_RESTORED, commentary));
                commentary.IsDeleted = false;
                await S.Db.SaveChangesAsync();

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                throw new Exception();
            }
        }
    }
}