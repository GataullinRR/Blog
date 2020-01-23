using System;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DBModels
{
    public class ModeratorsGroupStatistic : Statistic<ModeratorsGroup, ModeratorsGroupDayStatistic>
    {

    }

    public class ModeratorsGroupDayStatistic : IDayStatistic
    {
        [Key]
        public string Id { get; set; }
        public DateTime Day { get; set; }
        [Required]
        public virtual ModeratorsGroupStatistic Owner { get; set; }

        public int ResolvedEntitiesCount { get; set; }
        public TimeSpan SummedTimeToAssignation { get; set; }
        public TimeSpan SummedTimeFromAssignationToResolving { get; set; }
        public TimeSpan SummedResolveTime { get; set; }
    }
}
