using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
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
