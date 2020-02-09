using DBModels;
using System;
using System.Collections.Generic;

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

    public class ProfilePermissions
    {
        public bool CanSeeTabs { get; set; }
        public bool CanSeeSettingsTab { get; set; }
        public bool CanSeeActionsTab { get; set; }
        public bool CanBan { get; set; }
        public bool CanUnbanUser { get; set; }
        public bool CanReport { get; set; }
        public bool CanReportViolation { get; set; }
        public bool CanSeeGeneralInformation { get; set; }
        public bool CanSeePrivateInformation { get; set; }
        public bool CanSeeServiceInformation { get; set; }
        public bool CanEdit { get; set; }
        public bool CanChangePassword { get; set; }
        public bool CanChangeEMail { get; set; }
    }

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

    public class UserActionModel
    {
        public DateTime ActionDate { get; set; }
        public ActionType Type { get; set; }
        public object ActionObject { get; set; }
        public bool IsSelfAction { get; set; }
    }

    public class ViolationModel
    {
        public DateTime CreationTime { get; set; }
        public object Object { get; set; }
        public string Description { get; set; }
        public string Reporter { get; set; }
        public string ReporterId { get; set; }
    }

    public class ReportModel
    {
        public string Reporter { get; set; }
        public string ReporterId { get; set; }
        public DateTime CreationTime { get; set; }
        public object Object { get; set; }
    }
}
