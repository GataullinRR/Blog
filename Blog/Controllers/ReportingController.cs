using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Blog.Controllers
{
    public class ReportingController : ControllerBase
    {
        readonly ReportViewConfirmationTokenProvider _reportViewConfirmationTokens;

        public ReportingController(ServicesProvider serviceProvider) : base(serviceProvider)
        {
            _reportViewConfirmationTokens = Services.ServiceProvider.GetService<ReportViewConfirmationTokenProvider>();
        }

        public async Task<IActionResult> ReportProfileAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(Services.Db.ProfilesInfos.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IActionResult> ReportPostAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(Services.Db.Posts.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        public async Task<IActionResult> ReportCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(Services.Db.Commentaries.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        async Task<IActionResult> reportAsync(IReportObject reportObject)
        {
            await Services.Permissions.ValidateReportAsync(reportObject);

            Report report;
            var reportingUser = await Services.UserManager.GetUserAsync(User);
            if (reportObject is Post post)
            {
                report = new Report(reportingUser, post.Author, reportObject);
            }
            else if (reportObject is Profile profile)
            {
                var owner = await Services.Db.Users.FirstOrDefaultAsync(u => u.Profile.Id == profile.Id);
                report = new Report(reportingUser, owner, reportObject);
            }
            else if (reportObject is Commentary commentary)
            {
                report = new Report(reportingUser, commentary.Author, reportObject);
                commentary.IsHidden = commentary.IsHidden 
                    ? true 
                    : Services.Decisions.ShouldHide(commentary);
            }
            else
            {
                throw new InvalidOperationException("Can't create report for object of such type");
            }

            Services.Db.Reports.Add(report);
            reportingUser.Actions.Add(new UserAction(ActionType.REPORT, reportObject));
            if (Services.Decisions.ShouldReportToModerator(reportObject))
            {
                await Services.DbUpdator.EnsureHasEnoughModeratorsInChargeAsync(reportObject.Author);
                foreach (var moderator in reportObject.Author.ModeratorsInCharge)
                {
                    var alreadyAdded = moderator.ModeratorPanel.EntitiesToCheck
                        .FirstOrDefault(e => e.Entity == reportObject);
                    if (alreadyAdded == null)
                    {
                        moderator.ModeratorPanel.AddEntityToCheck(reportObject);
                    }
                    else
                    {
                        alreadyAdded.AddTime = DateTime.UtcNow;
                    }
                }
            }
            await Services.Db.SaveChangesAsync();

            LayoutModel.AddMessage("Report has been submitted");

            return Redirect(Services.History.GetLastURL());
        }
    }
}