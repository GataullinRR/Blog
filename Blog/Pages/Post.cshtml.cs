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
    public class PostModel : ExtendedPageModel
    {
        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }

        public PostModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Post = await DB.Posts.FirstOrDefaultAsync(p => p.Id == id);
            
            if (Post == null)
            {
                return RedirectToPage("/Index");
            }
            else
            {
                var currentUser = await UserManager.GetUserAsync(User);

                Commentaries = DB.Commentaries.Where(c => c.Post == Post);
                foreach (var commentary in Commentaries)
                {
                    commentary.ViewStatistic.UpdateStatistic(currentUser);
                }
                Post.ViewStatistic.UpdateStatistic(currentUser);

                var x = await DB.SaveChangesAsync();

                return Page();
            }
        }

        [HttpPost(), Authorize(Roles = Roles.NOT_RESTRICTED)]
        public async Task<IActionResult> OnPostAddCommentaryAsync(int postId, [Required(), MinLength(6), MaxLength(1000)]string commentBody)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await GetCurrentUserModelOrThrowAsync();
                if (commentBody != null)
                {
                    var comment = new Commentary(
                        await DB.Users.FirstAsync(u => u.UserName == User.Identity.Name), 
                        DateTime.UtcNow, 
                        await DB.Posts.FindAsync(postId), 
                        commentBody);
                    DB.Commentaries.Add(comment);
                    currentUser.Actions.Add(new UserAction(ActionType.ADD_COMMENTARY, comment.Id.ToString()));
                    await DB.SaveChangesAsync();
                }
            }
         
            return RedirectToPage("/Post", new { id = postId });
        }
    }
}