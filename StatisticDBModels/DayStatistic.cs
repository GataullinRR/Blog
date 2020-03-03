using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StatisticDBModels
{
    public interface IDayStatistic
    {
        DateTime Day { get; set; }
    }

    //public abstract class DayStatistic<T> : IDayStatistic
    //{
    //    [Key]
    //    public string Id { get; set; }
    //    public DateTime Day { get; set; }
    //    [Required]
    //    public T OwnerId { get; set; }
    //    [Required]
    //    public List<ActionStatistic<T>> Actions { get; set; } = new List<ActionStatistic<T>>();
    //}
}
