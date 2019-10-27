using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
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
    public class CommentaryController : ExtendedController
    {
        public CommentaryController(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryEditAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await DB.Commentaries.FirstAsync(c => c.Id == id);
                await Permissions.ValidateEditCommentaryAsync(commentary);

                return PartialView("_CommentaryEdit", commentary);
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpGet(), AllowAnonymous()]
        public async Task<IActionResult> GetCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await DB.Commentaries.FirstAsync(c => c.Id == id);
                var user = await UserManager.GetUserAsync(User);

                return PartialView("_Commentary", new CommentaryModel(user, commentary, Permissions));
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost()]
        public async Task<IActionResult> UpdateCommentaryAsync([Required]int id, [Required]string body, string editReason)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await UserManager.GetUserAsync(User);
                var commentary = await DB.Commentaries.FirstAsync(c => c.Id == id);
                await Permissions.ValidateEditCommentaryAsync(commentary);

                var user = await UserManager.GetUserAsync(User);
                commentary.Body = body;
                commentary.Edits.Add(new CommentaryEdit(user, editReason, DateTime.UtcNow));
                currentUser.Actions.Add(new UserAction(ActionType.COMMENTARY_EDIT, commentary.Id.ToString()));
                await DB.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = commentary.Post.Id });
            }
            else
            {
                throw new Exception();
            }
        }
    }
}