using System.Collections.Generic;

namespace Blog.Models
{
    public class ProfileModel
    {
        public ProfilePermissions Permissions { get; set; }
        public UserProfileModel TargetUser { get; set; }
        public bool IsCurrentUser { get; set; }
        /// <summary>
        /// Page is shown to the current logged in user
        /// </summary>
        public string ContactHelpEmail { get; set; }

        public IList<PostProfileModel> Posts { get; set; }
        public IList<UserActionModel> Actions { get; set; }
        public IList<ViolationModel> Violations { get; set; }
        public IList<ReportModel> Reports { get; set; }
        public IList<ReportModel> ReportedReports { get; set; }
        public ProfileImageModel ProfileImage { get; set; }
    }
}
