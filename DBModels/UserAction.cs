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
        POST_CREATED = 100,
        ADD_COMMENTARY = 200,
        PROFILE_EDIT = 300,
        POST_EDITED = 400,
        COMMENTARY_EDIT = 450,
        COMMENTARY_DELETE = 500,
        COMMENTARY_UNDELETE = 525,
        POST_DELETED = 550,
        POST_UNDELETED = 575,
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
        
        REPORT_CHECKED = 1400,
        VIOLATION_REPORTED = 1500,
        MARKED_AS_MODERATED = 2000,
        MARKED_AS_NOT_PASSED_MODERATION = 2100,
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
        public IReportable ReportObject => CommentaryObject ?? (IReportable)PostObject ?? ProfileObject;

        public UserAction() { }

        public UserAction(ActionType actionType, object @object)
        {
            ActionType = actionType;
            ActionDate = DateTime.UtcNow;
            CommentaryObject = @object as Commentary;
            PostObject = @object as Post;
            ProfileObject = @object as Profile;
        }
    }
}
