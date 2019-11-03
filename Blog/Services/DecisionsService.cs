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
            return commentary.ViewStatistic.TotalViews > 10
                && (commentary.Reports.Count() + 1) / (double)commentary.ViewStatistic.TotalViews > 0.1;
        }
        public bool ShouldReportToModerator(Commentary commentary)
        {
            return commentary.IsHidden;
        }
    }
}
