using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DBModels
{
    public class UserStatistic
    {
        [Key]
        public string Id { get; set; }
        [Required]
        public virtual User Owner { get; set; }

        [NotMapped]
        public virtual UserDayStatistic LastDayStatistic => DayStatistics.AsQueryable().OrderByDescending(s => s.Day).FirstOrDefault();
        [Required]
        public virtual List<UserDayStatistic> DayStatistics { get; set; } = new List<UserDayStatistic>();
    }

    public class UserDayStatistic : DayStatistic<UserStatistic>
    {

    }
}
