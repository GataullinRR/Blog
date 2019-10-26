using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DBModels;
using Microsoft.AspNetCore.Mvc;

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

            var reportingUser = await UserManager.GetUserAsync(User);
            if (reportObject is Post post)
            {
                DB.PostsReports.Add(new PostReport(reportingUser, post));
            }
            else if (reportObject is ProfileInfo profile)
            {
                DB.ProfilesReports.Add(new ProfileReport(reportingUser, profile));
            }
            else if (reportObject is Commentary commentary)
            {
                DB.CommentariesReports.Add(new CommentaryReport(reportingUser, commentary));
            }
            else
            {
                throw new InvalidOperationException("Can't create report for object of such type");
            }

            await DB.SaveChangesAsync();
            LayoutModel.Messages.Add("Report has been submitted");

            return Redirect(History.GetLastURL());
        }
    }
}