using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Pages;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class SearchController : ControllerBase
    {
        public static readonly string SEARCH_URI = getURIToAction(nameof(SearchController), nameof(Search));

        public SearchController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        public IActionResult Search(string query)
        {
            return RedirectToPage("/Index", "Filtered", new { filter = query });
        }
    }
}