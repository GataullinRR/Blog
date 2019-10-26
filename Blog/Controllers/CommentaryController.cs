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
    public class CommentaryController : Controller
    {
        readonly BlogContext _db;
        readonly PermissionsService _permissions;
        readonly UserManager<User> _userManager;

        public CommentaryController(BlogContext db, PermissionsService permissions, UserManager<User> userManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryEditAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var commentary = await _db.Commentaries
                    .Include(c => c.Author)
                    .FirstAsync(c => c.Id == id);
                await _permissions.ValidateEditCommentaryAsync(commentary);

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
                var commentary = await _db.Commentaries
                    .IncludeAuthor()
                    .FirstAsync(c => c.Id == id);
                var user = await _userManager.GetUserAsync(User);

                return PartialView("_Commentary", new CommentaryModel(user, commentary, _permissions));
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
                var commentary = await _db.Commentaries
                    .Include(c => c.Author)
                    .Include(c => c.Post)
                    .FirstAsync(c => c.Id == id);
                await _permissions.ValidateEditCommentaryAsync(commentary);

                var user = await _userManager.GetUserAsync(User);
                commentary.Body = body;
                commentary.Edits.Add(new CommentaryEdit(user, editReason, DateTime.UtcNow));
                await _db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = commentary.Post.Id });
            }
            else
            {
                throw new Exception();
            }
        }
    }
}