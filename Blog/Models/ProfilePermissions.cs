namespace Blog.Models
{
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
}
