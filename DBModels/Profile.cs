using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public class Profile : IDbEntity, IReportObject
    {
        [Key]
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Gender Gender { get; set; }
        public string Image { get; set; }
        public string About { get; set; } = "";
        [Required] public virtual ViewStatistic ViewStatistic { get; set; }
        [InverseProperty(nameof(Report.ProfileObject))]
        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();
        public string AuthorForeignKey { get; set; }
        public virtual User Author { get; set; }

        public Profile() { }

        public Profile(DateTime registrationDate)
        {
            RegistrationDate = registrationDate;
            ViewStatistic = new ViewStatistic();
        }
    }
}
