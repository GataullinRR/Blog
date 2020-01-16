using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    public class SearchController : ControllerBase
    {
        public static readonly string SEARCH_URI = getURIToAction(nameof(SearchController), nameof(Search));

        public SearchController(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        public IActionResult Search(string query)
        {
            return RedirectToPage("/Index", new { filter = query });
        }
    }
}