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

namespace Blog.Pages
{
    public class ViolationReportModel : PageModelBase
    {
        [Required, MinLength(3), MaxLength(500), BindProperty()]
        public string Description { get; set; }
        [Required, BindProperty]
        public string ReportObjectToken { get; set; }

        public ViolationReportModel(ServicesProvider services) : base(services)
        {

        }

        public async Task OnGetCommentary([Required]int id)
        {
            await initialize(async () => await S.Db.Commentaries.FirstOrDefaultByIdAsync(id));
        }

        public async Task OnGetPost([Required]int id)
        {
            await initialize(async () => await S.Db.Posts.FirstOrDefaultByIdAsync(id));
        }

        public async Task OnGetProfile([Required]int id)
        {
            await initialize(async () => await S.Db.ProfilesInfos.FirstOrDefaultByIdAsync(id));
        }

        async Task initialize(Func<Task<IModeratableObject>> getter)
        {
            if (ModelState.IsValid)
            {
                var reportObject = await getter();
                await Permissions.ValidateReportViolationAsync(reportObject);
                ReportObjectToken = reportObject.GetFinderToken().SerializeToString();
            }
            else
            {
                throw new Exception();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var reportObject = await S.Db.FindObjectByTokenOrNullAsync<IModeratableObject>(FinderToken.DeserializeFromString(ReportObjectToken));
                await Permissions.ValidateReportViolationAsync(reportObject);
                var currentUser = await S.Utilities.GetCurrentUserModelOrThrowAsync();
                var violation = new Violation(currentUser, reportObject.Author, reportObject, Description);
                reportObject.Violations.Add(violation);
                currentUser.Actions.Add(new UserAction(ActionType.VIOLATION_REPORTED, reportObject));
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