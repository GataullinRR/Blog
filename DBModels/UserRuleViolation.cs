using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DBModels
{
    public enum ViolationObjectType
    {
        UNSPECIFIED = 0,
        COMMENTARY = 100,
        POST = 1000,
    }

    // I wanted this row to be deleted when the user gets deleted, but not the reporter...
    public class UserRuleViolation : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required, InverseProperty(nameof(DBModels.User.Violations))]
        public virtual User User { get; set; }
        [InverseProperty(nameof(DBModels.User.ReportedViolations))]
        public virtual User Reporter { get; set; }
        [Required]
        public string Description { get; set; }
        public ViolationObjectType ObjectType { get; set; }
        public string ObjectId { get; set; }

        public UserRuleViolation() { }

        public UserRuleViolation(User user, User reporter, string description)
        {
            User = user ?? throw new ArgumentNullException(nameof(user));
            Reporter = reporter ?? throw new ArgumentNullException(nameof(reporter));
            Description = description ?? throw new ArgumentNullException(nameof(description));
        }
    }
}
