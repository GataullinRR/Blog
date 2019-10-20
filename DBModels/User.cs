using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Policy;

namespace DBModels
{
    public enum Gender
    {
        [Description("Unspecified")]
        UNSPECIFIED = 0,
        [Description("Male")]
        MALE = 100,
        [Description("Female")]
        FEMALE = 1000,
        [Description("Another")]
        ANOTHER = 10000,
    }

    public enum ViolationObjectType
    {
        UNSPECIFIED = 0,
        COMMENTARY = 100,
        POST = 1000,
    }

    public enum ProfileState
    {
        [Description("Restricted")]
        RESTRICTED = 0,
        [Description("Active")]
        ACTIVE = 100,
        [Description("Banned")]
        BANNED = 1000,
    }

    public class User : IdentityUser
    {
        [Required]
        public virtual ProfileInfo Profile { get; set; }
        [Required]
        public virtual ProfileStatus Status { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Commentary> Commentaries { get; set; }
        public virtual List<UserRuleViolation> Violations { get; set; } = new List<UserRuleViolation>();
        public virtual List<UserRuleViolation> ReportedViolations { get; set; } = new List<UserRuleViolation>();

        public User() { }

        public User(ProfileInfo info, ProfileStatus status)
        {
            Profile = info ?? throw new ArgumentNullException(nameof(info));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }
    }
}
