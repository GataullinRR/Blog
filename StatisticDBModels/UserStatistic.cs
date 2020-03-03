using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StatisticDBModels
{
    public class UserStatistic
    {
        [Key]
        public string UserId { get; set; }

        public ICollection<ActionStatistic<UserStatistic>> ActionsTotal { get; set; } = new List<ActionStatistic<UserStatistic>>();

        public UserStatistic()
        {

        }
        public UserStatistic(string userId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
        }
    }
}
