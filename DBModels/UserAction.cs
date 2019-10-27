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
        COMMENTARY_EDIT = 500,
        REPORT = 600,
        BAN = 700,
        UNBAN = 750,
        PASSWORD_RESET = 800,
        PASSWORD_CHANGED = 900,
        EMAIL_CONFIRMATION = 1000,
        SIGNED_IN = 1100,
        SIGNED_OUT = 1200,
        SIGN_IN_FAIL = 1300
    }

    public class UserAction
    {
        [Key]
        public int Id { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime ActionDate { get; set; }
        public string ObjectId { get; set; }

        public UserAction() { }

        public UserAction(ActionType actionType, string objectId)
            :this(actionType, DateTime.UtcNow, objectId)
        {

        }
        public UserAction(ActionType actionType, DateTime actionDate, string objectId)
        {
            ActionType = actionType;
            ActionDate = actionDate;
            ObjectId = objectId;
        }
    }
}
