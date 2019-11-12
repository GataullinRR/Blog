using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public enum ActionType
    {
        NONE = 0,
        OTHER = 50,
        POST_CREATE = 100,
        ADD_COMMENTARY = 200,
        PROFILE_EDIT = 300,
        POST_EDIT = 400,
        COMMENTARY_EDIT = 450,
        COMMENTARY_DELETE = 500,
        REPORT = 600,
        BAN = 700,
        UNBAN = 750,
        PASSWORD_RESETING = 800,
        PASSWORD_RESET = 850,
        PASSWORD_CHANGED = 900,
        PASSWORD_CHANGING = 950,
        EMAIL_CONFIRMED = 1000,
        EMAIL_CHANGING = 1025,
        EMAIL_CHANGED = 1050,
        SIGNED_IN = 1100,
        SIGNED_OUT = 1200,
        SIGN_IN_FAIL = 1300,
        
        REPORT_CHECKED = 1400
    }

    public class UserAction : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public virtual Commentary CommentaryObject { get; set; }
        public virtual Post PostObject { get; set; }
        public virtual Profile ProfileObject { get; set; }
        public IReportObject ReportObject => CommentaryObject ?? (IReportObject)PostObject ?? ProfileObject;

        public UserAction() { }

        public UserAction(ActionType actionType, object actionObject)
        {
            ActionType = actionType;
            ActionDate = DateTime.UtcNow;
            CommentaryObject = actionObject as Commentary;
            PostObject = actionObject as Post;
            ProfileObject = actionObject as Profile;
        }
    }
}
