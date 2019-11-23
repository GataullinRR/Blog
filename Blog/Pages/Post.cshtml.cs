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
using Utilities.Extensions;

namespace Blog.Pages
{
    public class PostModel : PageModelBase
    {
        public bool ShowLastEdit { get; private set; }
        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }

        public PostModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync([Required]int id, bool? lastEdit)
        {
            if (ModelState.IsValid)
            {
                ShowLastEdit = lastEdit.NullToFalse();
                Post = await S.Db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (Post == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    await S.Permissions.ValidateViewPostAsync(Post, ShowLastEdit);
                    var currentUser = await S.UserManager.GetUserAsync(User);

                    Commentaries = S.Db.Commentaries.Where(c => c.Post == Post);
                    foreach (var commentary in Commentaries)
                    {
                        commentary.ViewStatistic.UpdateStatistic(currentUser);
                    }
                    Post.ViewStatistic.UpdateStatistic(currentUser);

                    var x = await S.Db.SaveChangesAsync();

                    return Page();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddCommentaryAsync([Required]int postId, [Required(), MinLength(6), MaxLength(1000)]string commentBody)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(postId);
                await Permissions.ValidateAddCommentaryAsync(post);
                if (commentBody != null)
                {
                    var comment = new Commentary(
                        await S.Db.Users.FirstAsync(u => u.UserName == User.Identity.Name), 
                        DateTime.UtcNow, 
                        await S.Db.Posts.FindAsync(postId), 
                        commentBody);
                    S.Db.Commentaries.Add(comment);
                    await S.Db.SaveChangesAsync();
                    currentUser.Actions.Add(new UserAction(ActionType.ADD_COMMENTARY, comment));
                    await S.Db.SaveChangesAsync();
                }
            }
         
            return RedirectToPage("/Post", new { id = postId });
        }
    }
}