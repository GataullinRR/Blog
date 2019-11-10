using System.Collections.Generic;

namespace DBModels
{
    public interface IReportObject : IDbEntity
    {
        User Author { get; }
        ViewStatistic ViewStatistic { get; }
        List<Report> Reports { get; }
    }
}
