using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace StatisticDBModels
{
    public class ModeratorsGroupStatistic : Statistic<int, ModeratorsGroupDayStatistic>
    {

    }

    public class ModeratorsGroupDayStatistic : IDayStatistic
    {
        [Key]
        public string Id { get; set; }

        public DateTime Day { get; set; }
        public int ResolvedEntitiesCount { get; set; }
        public double SummedTimeToAssignation { get; set; }
        public double SummedTimeFromAssignationToResolving { get; set; }
        public double SummedResolveTime { get; set; }

        [Required]
        public virtual ModeratorsGroupStatistic Owner { get; set; }
    }
}
