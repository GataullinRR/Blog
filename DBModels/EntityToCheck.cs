using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public enum CheckReason
    {
        NOT_SET = 0,
        TOO_MANY_REPORTS = 100
    }

    public interface IEntityToCheck : IDbEntity
    {
        object Entity { get; }
        DateTime AddTime { get; }
        DateTime? AssignationTime { get; set; }
        DateTime? ResolvingTime { get; set; }
        bool IsResolved { get; }
        CheckReason CheckReason { get; set; }
        User AssignedModerator { get; set; }
        User EntityOwner { get; }
    }

    public class EntityToCheck<T> : IEntityToCheck
    {
        public EntityToCheck()
        {
        }

        public EntityToCheck(T entity)
        {
            Entity = entity;
            AddTime = DateTime.UtcNow;
        }

        [Key] public int Id { get; set; }
        [Required] public virtual T Entity { get; set; }
        public DateTime AddTime { get; set; }
        public DateTime? AssignationTime { get; set; }
        public DateTime? ResolvingTime { get; set; }
        public bool IsResolved => ResolvingTime != null;
        public CheckReason CheckReason { get; set; }
        object IEntityToCheck.Entity => Entity;
        public User AssignedModerator { get; set; }
        public User EntityOwner { get; set; }
    }
}
