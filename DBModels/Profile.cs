using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class Profile : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Gender Gender { get; set; }
        public string Image { get; set; }
        public string About { get; set; } = "";
        [Required] public virtual ViewStatistic ViewStatistic { get; set; }

        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();

        public Profile() { }

        public Profile(DateTime registrationDate)
        {
            RegistrationDate = registrationDate;
            ViewStatistic = new ViewStatistic();
        }
    }
}
