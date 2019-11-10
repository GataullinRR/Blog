using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class DecisionsService : ServiceBase
    {
        public DecisionsService(ServicesProvider serviceProvider) : base(serviceProvider)
        {

        }

        public bool ShouldHide(Commentary commentary)
        {
            return ShouldReportToModerator(commentary);
        }

        public bool ShouldReportToModerator(IReportObject reportObject)
        {
            return reportObject.ViewStatistic.TotalViews >= 10
                && (reportObject.Reports.Count()) / (double)reportObject.ViewStatistic.TotalViews > 0.099;
        }
    }
}
