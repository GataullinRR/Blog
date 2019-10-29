using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    public class ReportingController : ExtendedController
    {
        public ReportingController(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            PersistLayoutModel = true;
        }

        public async Task<IActionResult> ReportProfileAsync([Required]int id)
        {
            if (ModelState.IsValid)
            {
                return await reportAsync(DB.ProfilesInfos.FirstOrDefault(p => p.Id == id));
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
                return await reportAsync(DB.Posts.FirstOrDefault(p => p.Id == id));

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
                return await reportAsync(DB.Commentaries.FirstOrDefault(p => p.Id == id));
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        async Task<IActionResult> reportAsync(object reportObject)
        {
            await Permissions.ValidateReportAsync(reportObject);

            Report report;
            var reportingUser = await UserManager.GetUserAsync(User);
            if (reportObject is Post post)
            {
                report = new Report(reportingUser, post.Author, ReportObjectType.POST, post.Id, DateTime.UtcNow);
                post.Reports.Add(report);
            }
            else if (reportObject is Profile profile)
            {
                var owner = await DB.Users.FirstOrDefaultAsync(u => u.Profile.Id == profile.Id);
                report = new Report(reportingUser, owner, ReportObjectType.PROFILE, profile.Id, DateTime.UtcNow);
                profile.Reports.Add(report);
            }
            else if (reportObject is Commentary commentary)
            {
                report = new Report(reportingUser, commentary.Author, ReportObjectType.COMMENTARY, commentary.Id, DateTime.UtcNow);
                commentary.Reports.Add(report);
                commentary.IsHidden = commentary.IsHidden 
                    ? true 
                    : commentary.ViewStatistic.TotalViews > 10
                      && (commentary.Reports.Count() + 1) / (double)commentary.ViewStatistic.TotalViews > 0.1;
            }
            else
            {
                throw new InvalidOperationException("Can't create report for object of such type");
            }

            await DB.SaveChangesAsync();
            reportingUser.Actions.Add(new UserAction(ActionType.REPORT, report.Id.ToString()));
            await DB.SaveChangesAsync();

            LayoutModel.Messages.Add("Report has been submitted");

            return Redirect(History.GetLastURL());
        }
    }
}