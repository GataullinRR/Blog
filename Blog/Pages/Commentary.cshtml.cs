using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Models;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Blog.Pages
{
    public class CommentaryModel : PageModelBase
    {
        public Commentary Commentary { get; private set; }   
        public User CurrentUserModel { get; private set; }

        public CommentaryModel(ServicesLocator services) : base(services)
        {

        }

        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                CurrentUserModel = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                Commentary = await S.Db.Commentaries.FirstOrDefaultByIdAsync(id);

                return Page();
            }
            else
            {
                return Redirect(S.History.GetLastURL());
            }
        }
    }
}