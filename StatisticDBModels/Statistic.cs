using CommonDBModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace StatisticDBModels
{
    public class Statistic<TOwnerId, TDayStatistic>
        where TDayStatistic : IDayStatistic
    {
        [Key]
        public TOwnerId OwnerId { get; set; }

        [NotMapped]
        public TDayStatistic LastDayStatistic => DayStatistics
            .OrderByDescending(s => s.Day)
            .FirstOrDefault();
        [Required]
        public ICollection<TDayStatistic> DayStatistics { get; set; } = new List<TDayStatistic>();

        public Statistic()
        {

        }
    }
}
