using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DBModels
{
    public interface IDayStatistic
    {
        DateTime Day { get; set; }
    }

    public abstract class DayStatistic<T> : IDayStatistic
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
