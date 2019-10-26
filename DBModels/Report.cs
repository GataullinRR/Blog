using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public enum ReportObjectType
    {
        PROFILE = 0,
        POST = 100,
        COMMENTARY = 1000,
    }

    public class Report
    {
        [Key]
        public int Id { get; set; }
        [Required, InverseProperty(nameof(User.ReportedReports))]
        public virtual User Reporter { get; set; }
        [InverseProperty(nameof(User.Reports))]
        public virtual User ReportObjectOwner { get; set; }
        public virtual DateTime CreationDate { get; set; }
        public int ReportObjectId { get; set; }
        public ReportObjectType ReportObjectType { get; set; }

        public Report() { }

        public Report(User reporter, User reportObjectOwner, ReportObjectType reportObjectType, int reportObjectId, DateTime creationDate)
        {
            Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            ReportObjectOwner = reportObjectOwner ?? throw new ArgumentNullException(nameof(reportObjectOwner));
            ReportObjectType = reportObjectType;
            ReportObjectId = reportObjectId;
            CreationDate = creationDate;
        }
    }
}
