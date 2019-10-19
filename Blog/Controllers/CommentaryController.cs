using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [Authorize(Roles = Roles.NOT_RESTRICTED)]
    public class CommentaryController : Controller
    {
        readonly BlogContext _db;
        readonly PermissionsService _permissions;

        public CommentaryController(BlogContext db, PermissionsService permissions)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _permissions = permissions ?? throw new ArgumentNullException(nameof(permissions));
        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryEditAsync([Required]int id)
        {
            var commentary = await _db.Commentaries
                .Include(c => c.Author)
                .FirstAsync(c => c.Id == id);
            _permissions.ValidateEditCommentary(User, commentary);

            return PartialView("_CommentaryEdit", commentary);
        }

        [HttpGet()]
        public async Task<IActionResult> GetCommentaryAsync([Required]int id)
        {
            var commentary = await _db.Commentaries
                .Include(c => c.Author)
                .FirstAsync(c => c.Id == id);

            return PartialView("_Commentary", commentary);
        }

        [HttpPost()]
        public async Task UpdateCommentaryAsync([Required]int id, [Required]string body)
        {
            if (ModelState.IsValid)
            {
                var commentary = await _db.Commentaries
                    .Include(c => c.Author)
                    .FirstAsync(c => c.Id == id);
                _permissions.ValidateEditCommentary(User, commentary);

                commentary.Body = body;
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new Exception();
            }
        }
    }
}