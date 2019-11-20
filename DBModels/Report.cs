using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public class Report : ReportBase
    {
        public Report() { }

        public Report(User reporter, User reportObjectOwner, object reportObject) : base(reporter, reportObjectOwner, reportObject)
        {

        }
    }
}
