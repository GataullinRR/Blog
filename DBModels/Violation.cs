using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Utilities.Extensions;

namespace DBModels
{
    // I wanted this row to be deleted when the user gets deleted, but not the reporter...
    public class Violation : ReportBase
    {
        [Required]
        public string Description { get; set; }

        public Violation() { }

        public Violation(User reporter, User reportObjectOwner, IReportable reportObject, string description) : base(reporter, reportObjectOwner, reportObject)
        {
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
