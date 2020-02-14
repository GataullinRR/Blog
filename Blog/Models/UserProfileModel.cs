using DBModels;
using System;

namespace Blog.Models
{
    public class UserProfileModel
    {
        public bool IsEmailConfirmed { get; set; }
        public string UserId { get; set; }
        public string User { get; set; }
        public ProfileState State { get; set; }
        public string EMail { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string About { get; set; }
        public string Role { get; set; }
        public IViewStatistic ViewStatistic { get; set; }
    }
}
