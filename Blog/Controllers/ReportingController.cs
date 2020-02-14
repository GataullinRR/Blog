using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Blog.Attributes;
using Blog.Misc;
using Blog.Services;
using DBModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utilities.Extensions;

namespace Blog.Controllers
{
    [Authorize]
    public class ReportingController : ControllerBase
    {
        public ReportingController(ServiceLocator serviceProvider) : base(serviceProvider)
        {

        }

        [HttpPost, AJAX]
        public async Task<IActionResult> ReportProfileAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(S.Db.ProfilesInfos.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [HttpPost, AJAX]
        public async Task<IActionResult> ReportPostAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(S.Db.Posts.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [HttpPost, AJAX]
        public async Task<IActionResult> ReportCommentaryAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(S.Db.Commentaries.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        async Task<IActionResult> reportAsync(IReportable reportObject)
        {
            await S.Permissions.ValidateReportAsync(reportObject);

            Report report;
            var reportingUser = await S.UserManager.GetUserAsync(User);
            if (reportObject is Post post)
            {
                report = new Report(reportingUser, post.Author, reportObject);
            }
            else if (reportObject is Profile profile)
            {
                var owner = await S.Db.Users.FirstOrDefaultAsync(u => u.Profile.Id == profile.Id);
                report = new Report(reportingUser, owner, reportObject);
            }
            else if (reportObject is Commentary commentary)
            {
                report = new Report(reportingUser, commentary.Author, reportObject);
                commentary.IsHidden = commentary.IsHidden 
                    ? true 
                    : S.Decisions.ShouldHide(commentary);
            }
            else
            {
                throw new InvalidOperationException("Can't create report for object of such type");
            }

            S.Db.Reports.Add(report);
            await S.Repository.AddUserActionAsync(reportingUser, new UserAction(ActionType.REPORT, reportObject));
            if (S.Decisions.ShouldReportToModerator(reportObject))
            {
                var moderators = reportObject.Author.ModeratorsInChargeGroup;
                var alreadyAdded = moderators.EntitiesToCheck.FirstOrDefault(e => e.Entity == reportObject);
                if (alreadyAdded == null)
                {
                    moderators.AddEntityToCheck(reportObject, CheckReason.TOO_MANY_REPORTS);
                }
                else
                {
                    alreadyAdded.AddTime = DateTime.UtcNow;
                }
            }
            await S.Db.SaveChangesAsync();

            LayoutModel.AddMessage("Report has been submitted");

            return Redirect(S.History.GetLastURL());
        }
    }
}