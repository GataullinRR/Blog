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
    public class PostModel : PageModelBase
    {
        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }

        public PostModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await Services.Db.Posts.FirstOrDefaultAsync(p => p.Id == id);
            
            if (Post == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                var currentUser = await Services.UserManager.GetUserAsync(User);

                Commentaries = Services.Db.Commentaries.Where(c => c.Post == Post);
                foreach (var commentary in Commentaries)
                {
                    commentary.ViewStatistic.UpdateStatistic(currentUser);
                }
                Post.ViewStatistic.UpdateStatistic(currentUser);

                var x = await Services.Db.SaveChangesAsync();

                return Page();
            }
        }

        [HttpPost(), Authorize(Roles = Roles.NOT_RESTRICTED)]
        public async Task<IActionResult> OnPostAddCommentaryAsync(int postId, [Required(), MinLength(6), MaxLength(1000)]string commentBody)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
                await Permissions.ValidateAddCommentaryAsync();
                if (commentBody != null)
                {
                    var comment = new Commentary(
                        await Services.Db.Users.FirstAsync(u => u.UserName == User.Identity.Name), 
                        DateTime.UtcNow, 
                        await Services.Db.Posts.FindAsync(postId), 
                        commentBody);
                    Services.Db.Commentaries.Add(comment);
                    await Services.Db.SaveChangesAsync();
                    currentUser.Actions.Add(new UserAction(ActionType.ADD_COMMENTARY, comment));
                    await Services.Db.SaveChangesAsync();
                }
            }
         
            return RedirectToPage("/Post", new { id = postId });
        }
    }
}