using System;
using System.Text;

namespace DBModels
{
    public class ModeratorsGroupStatistic : Statistic<ModeratorsGroup, ModeratorsGroupDayStatistic>
    {

    }

    public class ModeratorsGroupDayStatistic : DayStatistic<ModeratorsGroupStatistic>
    {
        public TimeSpan AvgTimeToAssignation { get; set; }
        public TimeSpan AvgTimeFromAssignationToResolving { get; set; }
    }
}
