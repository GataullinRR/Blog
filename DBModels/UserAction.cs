using System;
using System.Collections.Generic;
using System.Text;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ComponentModel;

namespace DBModels
{
    public enum ActionType
    {
        [Description("Unknown")]
        NONE = 0,
        [Description("Other")]
        OTHER = 50,
        [Description("Post created")]
        POST_CREATED = 100,
        [Description("Commentary added")]
        COMMENTARY_ADDED = 200,
        [Description("Profile edited")]
        PROFILE_EDITED = 300,
        [Description("Post edited")]
        POST_EDITED = 400,
        [Description("Commentary edited")]
        COMMENTARY_EDIT = 450,
        [Description("Commentary deleted")]
        COMMENTARY_DELETE = 500,
        [Description("Commentary undeleted")]
        COMMENTARY_UNDELETE = 525,
        [Description("Post deleted")]
        POST_DELETED = 550,
        [Description("Post undeleted")]
        POST_UNDELETED = 575,
        [Description("Report submitted")]
        REPORT = 600,
        [Description("User banned")]
        BANNED = 700,
        [Description("User unbanned")]
        UNBANED = 750,
        [Description("Password resetting")]
        PASSWORD_RESETING = 800,
        [Description("Password reset")]
        PASSWORD_RESET = 850,
        [Description("Password changed")]
        PASSWORD_CHANGED = 900,
        [Description("Password changing")]
        PASSWORD_CHANGING = 950,
        [Description("E-mail confirmed")]
        EMAIL_CONFIRMED = 1000,
        [Description("E-mail changing")]
        EMAIL_CHANGING = 1025,
        [Description("E-mail changed")]
        EMAIL_CHANGED = 1050,
        [Description("Signed in")]
        SIGNED_IN = 1100,
        [Description("Signed out")]
        SIGNED_OUT = 1200,
        [Description("Sign in failed")]
        SIGN_IN_FAILED = 1300,
        
        [Description("Entity resolved")]
        ENTITY_RESOLVED = 1400,
        [Description("Violation reported")]
        VIOLATION_REPORTED = 1500,
        [Description("Marked as moderated")]
        MARKED_AS_MODERATED = 2000,
        [Description("Marked as not passed moderation")]
        MARKED_AS_NOT_PASSED_MODERATION = 2100,
    }

    public class UserAction : IDbEntity, IAuthored
    {
        [Key]
        public int Id { get; set; }
        public virtual User Author { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public virtual Commentary CommentaryObject { get; set; }
        public virtual Post PostObject { get; set; }
        public virtual Profile ProfileObject { get; set; }
        public virtual User Owner { get; set; }
        public object ActionObject => new object[] { CommentaryObject, PostObject, ProfileObject, Owner }.SkipNulls().SingleOrDefault();

        public UserAction() { }

        public UserAction(ActionType actionType, object @object)
        {
            ActionType = actionType;
            ActionDate = DateTime.UtcNow;
            CommentaryObject = @object as Commentary;
            PostObject = @object as Post;
            ProfileObject = @object as Profile;
            Owner = @object as User;
        }
    }
}
