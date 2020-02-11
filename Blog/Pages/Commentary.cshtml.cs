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
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class CommentaryModel : PageModelBase
    {
        public Models.CommentaryModel Commentary { get; private set; }

        public CommentaryModel(ServiceLocator services) : base(services)
        {

        }

        public async Task<IActionResult> OnGetAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                var query = await S.Repository.GetCommentaryModelsAsync(
                    S.Db.Commentaries
                    .AsNoTracking()
                    .Where(c => c.Id == id));
                Commentary = query.SingleOrDefault();

                if (Commentary == null)
                {
                    throw new NotFoundException();
                }
                else
                {
                    return Page();
                }
            }
            else
            {
                return Redirect(S.History.GetLastURL());
            }
        }
    }
}