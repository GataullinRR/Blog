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
        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }
        
        [BindProperty]
        public CommentaryCreateModel NewCommentary { get; set; }

        public PostModel(ServicesProvider serviceProvider) : base(serviceProvider)
        {
            NewCommentary = new CommentaryCreateModel();
        }

        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                Post = await S.Db.Posts.FirstOrDefaultAsync(p => p.Id == id);
                if (Post == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    await S.Permissions.ValidateViewPostAsync(Post);
                    var currentUser = await S.UserManager.GetUserAsync(User);

                    Commentaries = S.Db.Commentaries.Where(c => c.Post == Post);
                    foreach (var commentary in Commentaries)
                    {
                        await S.DbUpdator.UpdateViewStatisticAsync(currentUser, commentary.ViewStatistic);
                    }
                    await S.DbUpdator.UpdateViewStatisticAsync(currentUser, Post.ViewStatistic);

                    var x = await S.Db.SaveChangesAsync();
                    NewCommentary.PostId = id;

                    return Page();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddCommentaryAsync()
        {
            if (ModelState.IsValid)
            {
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var post = await S.Db.Posts.FirstOrDefaultByIdAsync(NewCommentary.PostId);
                await Permissions.ValidateAddCommentaryAsync(post);
                if (NewCommentary.Body != null)
                {
                    var comment = new Commentary(
                        await S.Db.Users.FirstAsync(u => u.UserName == User.Identity.Name),
                        DateTime.UtcNow,
                        await S.Db.Posts.FindAsync(NewCommentary.PostId),
                        NewCommentary.Body);
                    S.Db.Commentaries.Add(comment);
                    await S.Db.SaveChangesAsync();
                    currentUser.Actions.Add(new UserAction(ActionType.COMMENTARY_ADDED, comment));
                    await S.Db.SaveChangesAsync();
                }

                return RedirectToPage("/Post", new { id = NewCommentary.PostId });
            }
            else
            {
                return Redirect(S.History.GetLastURL());
            }
        }
    }
}