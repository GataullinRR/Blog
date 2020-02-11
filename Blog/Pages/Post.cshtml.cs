using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Middlewares;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Utilities.Extensions;
using Utilities.Types;

namespace Blog.Pages
{
    public class PostModel : PageModelBase
    {
        public Models.PostModel Post { get; private set; } 

        [BindProperty]
        public CommentaryCreateModel NewCommentary { get; set; }

        public PostModel(ServiceLocator serviceProvider) : base(serviceProvider)
        {
            NewCommentary = new CommentaryCreateModel();
        }

        [CustomResponseCacheHandler(CacheManagerService.POST_GET_CACHE_KEY)]
        public static async Task HandelCachedOnGetAsync(CacheScope scope)
        {
            var services = scope.ServiceProvider.GetService<ServiceLocator>();
            var viewStatistics = scope.RequestData.To<IViewStatistic[]>();

            await updateViewStatisticAsync(services.DbUpdator, false, viewStatistics);
        }

        [CustomResponseCache(20, 3 * 60, CacheMode.FOR_ANONYMOUS, CacheManagerService.POST_GET_CACHE_KEY)]
        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var post = await S.Db.Posts
                    .AsNoTracking()
                    .Include(p => p.Author)
                    .ThenInclude(a => a.Profile)
                    .Include(p => p.ModerationInfo)
                    .Include(p => p.ViewStatistic)
                    .Include(p => p.Edits)
                    .ThenInclude(e => e.Author)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (post == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    await S.Permissions.ValidateViewPostAsync(post);

                    var commentaries = await S.Repository.GetCommentaryModelsAsync(
                            S.Db.Commentaries
                            .AsNoTracking()
                            .Where(c => c.Post.Id == id));
                    Post = new Models.PostModel()
                    {
                        Author = post.Author.UserName,
                        AuthorId = post.Author.Id,
                        Body = post.Body,
                        PostId = post.Id,
                        CreationTime = post.CreationTime,
                        Title = post.Title,
                        CommentarySectionModel = new CommentarySectionModel()
                        {
                            Commentaries = commentaries.ToArray()
                        },
                        Edits = post.Edits.Select(e => new Models.PostEditModel()
                        {
                            Author = e.Author.UserName,
                            Reason = e.Reason,
                            AuthorId = e.Author.Id,
                            EditTime = e.EditTime
                        }).ToArray(),
                        AuthorBiography = post.Author.Profile.About,
                        IsAuthentificated = S.HttpContext.User?.Identity?.IsAuthenticated ?? false,
                        TotalViews = post.ViewStatistic.TotalViews,
                        ModerationState = post.ModerationInfo.State,
                        AuthorProfileImage = new ProfileImageModel()
                        {
                            RelativeUri = post.Author.Profile.Image
                        },
                        IsDeleted = post.IsDeleted,
                        DeleteReason = post.DeleteReason,

                        CanAddCommentary = await S.Permissions.CanAddCommentaryAsync(post),
                        CanDelete = await S.Permissions.CanDeletePostAsync(post),
                        CanRestore = await S.Permissions.CanRestorePostAsync(post),
                        CanReport = await S.Permissions.CanReportAsync(post),
                        CanReportViolation = await S.Permissions.CanReportViolationAsync(post),
                        CanEdit = await S.Permissions.CanEditPostAsync(post),
                        CanMarkAsModerated = await S.Permissions.CanMarkAsModeratedAsync(post),
                        CanMarkAsNotPassedModeration = await S.Permissions.CanMarkAsNotPassedModerationAsync(post),
                    };

                    var viewStatistics = new Enumerable<IViewStatistic>
                        {
                            post.ViewStatistic,
                            commentaries.Select(c => c.ViewStatistic),
                        }.ToArray();

                    await S.CacheManager.CacheManager.SetRequestDataAsync(CacheManagerService.POST_GET_CACHE_KEY, viewStatistics);
                    await updateViewStatisticAsync(S.DbUpdator, S.HttpContext.User?.Identity?.IsAuthenticated ?? false, viewStatistics);

                    NewCommentary.PostId = id;

                    return Page();
                }
            }
            else
            {
                throw new Exception();
            }
        }

        static async Task updateViewStatisticAsync(DbEntitiesUpdateService updateService, bool isUserRegistered, IViewStatistic[] viewStatistics)
        {
            foreach (var vs in viewStatistics)
            {
                await updateService.UpdateViewStatisticAsync(isUserRegistered, vs);
            }
        }

        [HttpPost]
        public async Task<IActionResult> OnPostAddCommentaryAsync()
        {
            if (ModelState.IsValid)
            {
                var post = await S.Db.Posts
                    .Include(p => p.ModerationInfo)
                    .FirstOrDefaultAsync(p => p.Id == NewCommentary.PostId);
                await Permissions.ValidateAddCommentaryAsync(post);

                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var comment = new Commentary(
                        currentUser,
                        DateTime.UtcNow,
                        post,
                        NewCommentary.Body);
                S.Db.Commentaries.Add(comment);
                await S.Repository.AddUserActionAsync(currentUser, new UserAction(ActionType.COMMENTARY_ADDED, comment));
                await S.Db.SaveChangesAsync();

                return RedirectToPage("/Post", new { id = NewCommentary.PostId });
            }
            else
            {
                return Redirect(S.History.GetLastURL());
            }
        }
    }
}