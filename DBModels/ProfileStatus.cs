using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class ProfileStatus : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        public ProfileState State { get; set; }
        public string StateReason { get; set; } 
        public DateTime? BannedTill { get; set; }
        public DateTime? LastPasswordRestoreAttempt { get; set; }

        public ProfileStatus() { }

        public ProfileStatus(ProfileState state)
        {
            State = state;
        }
    }
}
