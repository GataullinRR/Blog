using CommonDBModels;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace StatisticDBModels
{
    public interface IActionStatistic : IDbEntity
    {
        int ActionType { get; set; }
        int Count { get; set; }
    }

    public class ActionStatistic<T> : IActionStatistic 
    {
        [Key]
        public int Id { get; set; }

        public int ActionType { get; set; }
        public int Count { get; set; }

        [Required]
        public T DayStatistic { get; set; }

        public ActionStatistic()
        {

        }
        public ActionStatistic(int actionType, int count)
        {
            ActionType = actionType;
            Count = count;
        }
    }
}
