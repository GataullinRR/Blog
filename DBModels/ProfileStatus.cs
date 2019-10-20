using System;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public class ProfileStatus
    {
        [Key]
        public int Id { get; set; }
        public ProfileState State { get; set; }
        public DateTime? BannedTill { get; set; }
        public DateTime? LastPasswordRestoreAttempt { get; set; }

        public ProfileStatus(ProfileState state)
        {
            State = state;
        }
    }
}
