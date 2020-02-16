using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Attributes;
using Blog.Middlewares;
using Blog.Middlewares.CachingMiddleware.Policies;
using Blog.Misc;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Blog.Pages
{
    public class IndexModel : PageModelBase
    {
        public const int NUM_OF_POSTS_ON_PAGE = 3;

        public List<PostIndexModel> Posts { get; private set; }
        public int NumOfPages { get; private set; }
        public int CurrentPage { get; private set; }
        public string SearchQuery { get; private set; }
        public int NumOfSearchResults { get; private set; }

        public IndexModel(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [ServerResponseCache(1 * 3600, CachePolicy.UNAUTHENTICATED_USER_SCOPED, CacheManagerService.INDEX_GET_CACHE_KEY)]
        public async Task<IActionResult> OnGetAsync(int? pageIndex)
        {
            return await OnGetFilteredAsync(pageIndex, null);
        }

        public async Task<IActionResult> OnGetFilteredAsync(int? pageIndex, string filter)
        {
            CurrentPage = pageIndex ?? 0;

            var viewablePosts = S.Db.Posts.AsNoTracking()
                .Where(p => p.ModerationInfo.State == ModerationState.MODERATED && !p.IsDeleted);

            if (filter != null)
            {
                var keyword = filter.Split(" ").FirstElementOrDefault("");
                viewablePosts = viewablePosts.Where(p => p.Title.Contains(keyword));
                SearchQuery = filter;
                NumOfSearchResults = await viewablePosts.CountAsync();
                NumOfPages = NumOfSearchResults / NUM_OF_POSTS_ON_PAGE + ((NumOfSearchResults % NUM_OF_POSTS_ON_PAGE == 0) ? 0 : 1);
            }

            Posts = await viewablePosts.OrderByDescending(p => p.CreationTime)
                .Skip(CurrentPage * NUM_OF_POSTS_ON_PAGE)
                .Take(NUM_OF_POSTS_ON_PAGE)
                .Select(p => new PostIndexModel()
                {
                    Author = p.Author.UserName,
                    AuthorId = p.Author.Id,
                    BodyPreview = p.BodyPreview,
                    CreationTime = p.CreationTime,
                    PostId = p.Id,
                    Title = p.Title
                }).ToListAsync();

            if (filter == null)
            {
                NumOfPages = await viewablePosts.CountAsync();
                NumOfPages = NumOfPages / NUM_OF_POSTS_ON_PAGE + ((NumOfPages % NUM_OF_POSTS_ON_PAGE == 0) ? 0 : 1);
            }

            if (Posts.Count == 0 && CurrentPage != 0)
            {
                return RedirectToPage(new { pageIndex = 0 });
            }
            else
            {
                return Page();
            }
        }
    }
}