using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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
        public virtual Profile Profile { get; set; }
        [Required]
        public virtual ProfileStatus Status { get; set; }

        public virtual List<Post> Posts { get; set; }
        public virtual List<Commentary> Commentaries { get; set; }

        [InverseProperty(nameof(Violation.Reporter))]
        [Required] public virtual List<Violation> ReportedViolations { get; set; } = new List<Violation>();
        [InverseProperty(nameof(Violation.ObjectOwner))]
        [Required] public virtual List<Violation> Violations { get; set; } = new List<Violation>();

        [InverseProperty(nameof(Report.Reporter))]
        [Required] public virtual List<Report> ReportedReports { get; set; } = new List<Report>();
        [InverseProperty(nameof(Report.ObjectOwner))]
        [Required] public virtual List<Report> Reports { get; set; } = new List<Report>();
        
        [Required] public virtual List<UserAction> Actions { get; set; } = new List<UserAction>();

        /// <summary>
        /// Not null for User in <see cref="Roles.MODERATOR"/>
        /// </summary>
        public virtual ModeratorsGroup ModeratorsGroup { get; set; }
        /// <summary>
        /// Not null for each User
        /// </summary>
        public virtual ModeratorsGroup ModeratorsInChargeGroup { get; set; }

        public User() { }

        public User(Profile info, ProfileStatus status)
        {
            Profile = info ?? throw new ArgumentNullException(nameof(info));
            Status = status ?? throw new ArgumentNullException(nameof(status));
        }
    }
}
