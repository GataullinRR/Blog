using Blog.Attributes;
using DBModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Blog.Services
{
    [Service(ServiceType.SCOPED)]
    public class DecisionsService : ServiceBase
    {
        public DecisionsService(ServiceLocator serviceProvider) : base(serviceProvider)
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
    }
}
