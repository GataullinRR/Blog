namespace DBModels
{
    public interface IDeletable
    {
        bool IsDeleted { get; }
        string DeleteReason { get; }
    }
}
