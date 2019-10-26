using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class ProfileInfo
    {
        [Key]
        public int Id { get; set; }
        public DateTime RegistrationDate { get; set; }
        public Gender Gender { get; set; }
        public string Image { get; set; }
        public string About { get; set; } = "";

        [Required] public virtual List<ProfileReport> Reports { get; set; } = new List<ProfileReport>();

        public ProfileInfo(DateTime registrationDate)
        {
            RegistrationDate = registrationDate;
        }
    }
}
