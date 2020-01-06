using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace DBModels
{
    public interface IActionStatistic : IDbEntity
    {
        ActionType ActionType { get; set; }
        int Count { get; set; }
    }

    public class ActionStatistic<T> : IActionStatistic
    {
        [Key]
        public int Id { get; set; }
        public virtual DayStatistic<T> Owner { get; set; }

        public ActionType ActionType { get; set; }
        public int Count { get; set; }
    }
}
