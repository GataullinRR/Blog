using System.ComponentModel;

namespace DBModels
{
    public enum ModerationState
    {
        [Description("UNDER MODERATION")]
        UNDER_MODERATION = 0,
        [Description("MODERATION NOT PASSED")]
        MODERATION_NOT_PASSED = 1000,
        [Description("MODERATED")]
        MODERATED = 2000,
    }

    public interface IModeratable : IDbEntity, IAuthored
    {
        ModerationInfo ModerationInfo { get; set; }
    }
}
