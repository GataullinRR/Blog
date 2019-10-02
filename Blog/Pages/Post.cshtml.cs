using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class PostModel : PageModel
    {
        readonly BlogContext _db;

        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }

        public PostModel(BlogContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await _db.Posts.Include(p => p.Author).FirstOrDefaultAsync(p => p.Id == id);

            if (Post == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                Commentaries = _db.Commentaries
                    .Include(c => c.Post)
                    .Where(c => c.Post == Post)
                    .Include(c => c.Author);

                return Page();
            }
        }

        [HttpPost(), Authorize(Roles = Roles.NOT_RESTRICTED)]
        public async Task<IActionResult> OnPostAddCommentaryAsync(int postId, string commentBody)
        {
            if (commentBody != null)
            {
                var comment = new Commentary()
                {
                    Author = await _db.Users.FirstAsync(u => u.UserName == User.Identity.Name),
                    Body = commentBody,
                    Date = DateTime.Now,
                    Post = await _db.Posts.FindAsync(postId)
                };

                _db.Commentaries.Add(comment);
                await _db.SaveChangesAsync();
            }

            return RedirectToPage("/Post", new { id = postId });
        }
    }
}