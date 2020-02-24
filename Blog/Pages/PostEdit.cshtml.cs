using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Attributes;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Blog.Pages
{
    [Authorize]
    public class PostEditModel : PostCRUDPageModel
    {
        [BindProperty, Required(), MinLength(10), MaxLength(1000)]
        public string EditReason { get; set; }
        public Post Post { get; private set; }
        public bool CanEditPostTitle { get; private set; }

        public PostEditModel(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGetAsync([FromEntityId]IQueryable<Post> postQuery)
        {
            postQuery = postQuery.AsNoTracking();
            postQuery = await S.Permissions.CanEditPostTitleIncludeAsync(postQuery);
            // Will return 404 status code if there is no such post (because RequiredAttribute is present)
            var post = await postQuery.FirstOrDefaultAsync();

            Title = post.Title;
            Body = post.Body;
            Post = post;
            PostId = post.Id;
            CanEditPostTitle = await S.Permissions.CanEditPostTitleAsync(post);

            return Page();

            //if (ModelState.IsValid)
            //{
            //    var post = await S.Db.Posts
            //        .AsNoTracking()
            //        .Include(p => p.Author)
            //        .Include(p => p.ModerationInfo)
            //        .FirstOrDefaultAsync(p => p.Id == id);
            //    if (post != null)
            //    {
            //        Title = post.Title;
            //        Body = post.Body;
            //        Post = post;
            //        PostId = id;
            //        CanEditPostTitle = await S.Permissions.CanEditPostTitleAsync(post);

            //        return Page();
            //    }
            //    else
            //    {
            //        throw new NotFoundException();
            //    }
            //}
            //else
            //{
            //    throw new Exception();
            //}
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var editingPost = await S.Db.Posts
                    .Include(p => p.Edits)
                    .Include(p => p.ModerationInfo)
                    .Include(p => p.Author)
                    .FirstOrDefaultAsync(p => p.Id == PostId);
                await S.Permissions.ValidateEditPostAsync(editingPost);

                var authorId = await S.Utilities.GetCurrentUserIdOrThrowAsync();
                var author = await S.Db.Users
                    .Include(u => u.ModeratorsInChargeGroup)
                    .FirstOrDefaultAsync(u => u.Id == authorId);
                if (!await S.Permissions.CanEditPostTitleAsync(editingPost))
                {
                    Title = editingPost.Title;
                }
                var edit = new PostEdit(author, EditReason, DateTime.UtcNow, editingPost);
                var sanitizedBody = await S.Sanitizer.SanitizePostBodyAsync(Body);
                editingPost.Body = sanitizedBody;
                editingPost.Title = Title;
                editingPost.BodyPreview = getPostBodyPreview(sanitizedBody);
                editingPost.Edits.Add(edit);
                if (editingPost.ModerationInfo.State == ModerationState.MODERATION_NOT_PASSED)
                {
                    editingPost.ModerationInfo.State = ModerationState.UNDER_MODERATION;
                    editingPost.ModerationInfo.StateReasoning = "Post was edited";
                    author.ModeratorsInChargeGroup.AddEntityToCheck(editingPost, CheckReason.NEED_MODERATION);
                }
                else if (editingPost.ModerationInfo.State == ModerationState.MODERATED)
                {
                    author.ModeratorsInChargeGroup.AddEntityToCheck(editingPost, CheckReason.CHECK_REQUIRED);
                }
                await S.Repository.AddUserActionAsync(author, new UserAction(ActionType.POST_EDITED, editingPost));
                await S.Db.SaveChangesAsync();
                await S.CacheManager.ResetPostPageCacheAsync(PostId);

                LayoutModel.AddMessage("Changes applied!");

                return RedirectToPage("/Post", new { id = editingPost.Id });
            }
            else
            {
                return Page();
            }
        }
    }
}