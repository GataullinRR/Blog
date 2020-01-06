using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DBModels
{
    public class ModeratorsGroupStatistic : IDbEntity
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public virtual ModeratorsGroup Owner { get; set; }

        [NotMapped]
        public virtual ModeratorsGroupDayStatistic LastDayStatistic => DayStatistics
            .AsQueryable()
            .OrderByDescending(s => s.Day)
            .FirstOrDefault();
        [Required]
        public virtual List<ModeratorsGroupDayStatistic> DayStatistics { get; set; } = new List<ModeratorsGroupDayStatistic>();
    }

    public class ModeratorsGroupDayStatistic : DayStatistic<ModeratorsGroupStatistic>
    {
        public TimeSpan AvgTimeToAssignation { get; set; }
        public TimeSpan AvgTimeFromAssignationToResolving { get; set; }
    }

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

    public abstract class DayStatistic<T> 
    {
        [Key]
        public string Id { get; set; }
        public DateTime Day { get; set; }
        [Required]
        public virtual T Owner { get; set; }
        [Required]
        public virtual List<ActionStatistic<T>> Actions { get; set; } = new List<ActionStatistic<T>>();
    }
}
