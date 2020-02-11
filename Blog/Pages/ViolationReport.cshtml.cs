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
using Blog;
using Microsoft.EntityFrameworkCore;

namespace Blog.Pages
{
    public class ViolationReportModel : PageModelBase
    {
        [Required, MinLength(3), MaxLength(500), BindProperty()]
        public string Description { get; set; }

        [BindProperty]
        public int? CommentaryReportObjectId { get; set; }
        [BindProperty]
        public int? PostReportObjectId { get; set; }
        [BindProperty]
        public int? ProfileReportObjectId { get; set; }

        public ViolationReportModel(ServiceLocator services) : base(services)
        {

        }

        public void OnGetCommentary([Required]int id)
        {
            CommentaryReportObjectId = id;
        }

        public void OnGetPost([Required]int id)
        {
            PostReportObjectId = id;
        }

        public void OnGetProfile([Required]int id)
        {
            ProfileReportObjectId = id;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                IReportable reportObject;
                if (CommentaryReportObjectId != null)
                {
                    reportObject = await S.Db.Commentaries
                        .Include(o => o.Author)
                        .FirstAsync(c => c.Id == CommentaryReportObjectId.Value);
                }
                else if (PostReportObjectId != null)
                {
                    reportObject = await S.Db.Posts
                        .Include(o => o.Author)
                        .FirstAsync(c => c.Id == PostReportObjectId.Value);
                }
                else if (ProfileReportObjectId != null)
                {
                    reportObject = await S.Db.ProfilesInfos
                        .Include(o => o.Author)
                        .FirstAsync(c => c.Id == ProfileReportObjectId.Value);
                }
                else
                {
                    throw new NotSupportedException();
                }

                await Permissions.ValidateReportViolationAsync(reportObject);
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var violation = new Violation(currentUser, reportObject.Author, reportObject, Description);
                var action = new UserAction(ActionType.VIOLATION_REPORTED, reportObject) 
                {
                    Author = currentUser 
                };
                S.Db.UserRuleViolations.Add(violation);
                S.Db.UsersActions.Add(action);
                await S.Db.SaveChangesAsync();

                LayoutModel.AddMessage("Violation report has been submitted");

                return Redirect(S.History.GetLastURL());
            }
            else
            {
                return Page();
            }
        }
    }
}