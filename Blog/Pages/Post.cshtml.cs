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
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;
using Utilities.Types;

namespace Blog.Pages
{

    public class CacheModelScope
    {
        public IServiceProvider ServiceProvider { get; }
    }

    /// <summary>
    /// The marked method should be static, taking <see cref="CacheModelScope"/> and returning <see cref="Task{string}"/>, where <see cref="string"/>
    /// is JSON value of a <see cref="CustomCache.CACHE_MODEL_VAR"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class CacheModelProviderAttribute : Attribute
    {
        public CacheModelProviderAttribute(int invalidationInterval, string cacheKey)
        {
            InvalidationInterval = invalidationInterval;
            CacheKey = cacheKey ?? throw new ArgumentNullException(nameof(cacheKey));
        }

        /// <summary>
        /// In secounds
        /// </summary>
        public int InvalidationInterval { get; }
        public Key CacheKey { get; }
    }


    public class PostModel : PageModelBase
    {
        public Post Post { get; private set; }
        public IEnumerable<Commentary> Commentaries { get; private set; }
        
        [BindProperty]
        public CommentaryCreateModel NewCommentary { get; set; }

        public PostModel(ServicesLocator serviceProvider) : base(serviceProvider)
        {
            NewCommentary = new CommentaryCreateModel();
        }



        public class PostCacheModelDTO : ServerLayoutModel
        {
            
            public bool canEdit { get; set; }
            public bool canReport { get; set; }
            public bool canReportViolation { get; set; }
            public bool canMarkAsModerated { get; set; }
            public bool canMarkAsNotPassedModeration { get; set; }
            public bool canDelete { get; set; }
            public bool canRestore { get; set; }
        }

        [CacheModelProvider(1 * 60 * 60, CacheManagerService.POST_GET_CACHE_KEY)]
        public static async Task<string> ProvideModelAsync(CacheModelScope scope)
        {
            return "";
        }

        [CustomResponseCacheHandler(CacheManagerService.POST_GET_CACHE_KEY)]
        public static async Task HandelCachedOnGetAsync(CacheScope scope)
        {
            var services = scope.ServiceProvider.GetService<ServicesLocator>();
            var isUserRegistered = services.HttpContext.User?.Identity?.IsAuthenticated ?? false;
            var viewStatistics = scope.RequestData.To<IViewStatistic[]>();

            await updateViewStatisticAsync(services.DbUpdator, isUserRegistered, viewStatistics);
        }

        [CustomResponseCache(20, 3 * 60, CacheMode.PUBLIC, CacheManagerService.POST_GET_CACHE_KEY)]
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
                    var viewStatistics = new Enumerable<IViewStatistic>
                    {
                        Post.ViewStatistic,
                        Commentaries.Select(c => c.ViewStatistic),
                    }.ToArray();
                    
                    await S.CacheManager.CacheManager.SetRequestDataAsync(CacheManagerService.POST_GET_CACHE_KEY, viewStatistics);
                    await updateViewStatisticAsync(S.DbUpdator, currentUser != null, viewStatistics);

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