using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using DBModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class IndexModel : ExtendedPageModel
    {
        const int NUM_OF_POSTS_ON_PAGE = 3;

        public Post[] Posts { get; private set; }
        public int NumOfPages { get; private set; }
        public int CurrentPage { get; private set; }

        public IndexModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public async Task<IActionResult> OnGet(int? pageIndex)
        {
            CurrentPage = pageIndex ?? 0;
            Posts = await DB.Posts
                .OrderByDescending(p => p.CreationTime)
                .Skip(CurrentPage * NUM_OF_POSTS_ON_PAGE)
                .Take(NUM_OF_POSTS_ON_PAGE)
                .ToArrayAsync();
            NumOfPages = DB.Posts.Count();
            NumOfPages = NumOfPages / NUM_OF_POSTS_ON_PAGE + ((NumOfPages % NUM_OF_POSTS_ON_PAGE == 0) ? 0 : 1);

            if (Posts.Length == 0 && CurrentPage != 0)
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