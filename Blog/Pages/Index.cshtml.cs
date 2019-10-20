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
        public IEnumerable<Post> Posts => DB.Posts.Include(p => p.Author);

        public IndexModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {

        }

        public void OnGet()
        {

        }
    }
}