using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace DBModels
{
    public class Statistic<TOwner, TDayStatistic> : IDbEntity
        where TDayStatistic : IDayStatistic
    {
        public Statistic()
        {

        }

        [Key]
        public int Id { get; set; }
        [Required]
        public virtual TOwner Owner { get; set; }

        [NotMapped]
        public virtual TDayStatistic LastDayStatistic => DayStatistics
            .AsQueryable()
            .OrderByDescending(s => s.Day)
            .FirstOrDefault();
        [Required]
        public virtual List<TDayStatistic> DayStatistics { get; set; } = new List<TDayStatistic>();
    }
}
