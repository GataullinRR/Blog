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

        public CommentaryModel(ServicesProvider services) : base(services)
        {

        }

        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                CurrentUserModel = await Services.Utilities.GetCurrentUserModelOrThrowAsync();
                Commentary = await Services.Db.Commentaries.FirstOrDefaultByIdAsync(id);

                return Page();
            }
            else
            {
                return Redirect(Services.History.GetLastURL());
            }
        }
    }
}