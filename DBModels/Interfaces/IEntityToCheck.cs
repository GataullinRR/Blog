using System;
using System.ComponentModel;

namespace DBModels
{
    public enum CheckReason
    {
        [Description("Unknown")]
        NOT_SET = 0,
        [Description("Too many reports")]
        TOO_MANY_REPORTS = 1000,
        [Description("Moderation required")]
        NEED_MODERATION = 2000,
        [Description("Check required")]
        CHECK_REQUIRED = 3000
    }

    public interface IEntityToCheck : IDbEntity
    {
        object Entity { get; }
        DateTime AddTime { get; set; }
        DateTime? AssignationTime { get; set; }
        DateTime? ResolvingTime { get; set; }
        bool IsResolved { get; }
        CheckReason CheckReason { get; set; }
        User AssignedModerator { get; set; }
        User EntityOwner { get; }
    }
}
