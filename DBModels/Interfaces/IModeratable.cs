namespace DBModels
{
    public enum ModerationState
    {
        UNDER_MODERATION = 0,
        MODERATION_NOT_PASSED = 1000,
        MODERATED = 2000,
    }

    public interface IModeratable : IDbEntity, IAuthored
    {
        ModerationState State { get; set; }
    }
}
