using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    public class DecisionsService : ServiceBase
    {
        public DecisionsService(ServicesLocator serviceProvider) : base(serviceProvider)
        {

        }

        public bool ShouldHide(Commentary commentary)
        {
            return ShouldReportToModerator(commentary);
        }

        public bool ShouldReportToModerator(IReportable reportObject)
        {
            return reportObject.ViewStatistic.TotalViews >= 10
                && (reportObject.Reports.Count()) / (double)reportObject.ViewStatistic.TotalViews > 0.099;
        }

        //public async Task<bool> ShouldBeModeratedAsync(object reportObject)
        //{
        //    var currentUser = await S.UserManager.GetUserAsync(S.HttpContext.User);
        //    if (currentUser == null)
        //    {
        //        throw new UnauthorizedAccessException();
        //    }
        //    else
        //    {
        //        var t = reportObject.GetType();
        //        if (await S.UserManager.IsInOneOfTheRolesAsync(currentUser, Roles.GetAllNotLess(Roles.MODERATOR)))
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            if (!t.IsOneOf(typeof(PostEdit), typeof(Post), typeof(Profile), typeof(Commentary), typeof(CommentaryEdit)))
        //            {
        //                throw new NotSupportedException();
        //            }
        //            else
        //            {
        //                return t == typeof(PostEdit)
        //                    || t == typeof(Post)
        //                    || t == typeof(Profile);
        //            } 
        //        }
        //    }
        //}
    }
}
