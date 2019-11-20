using System.Collections.Generic;

namespace DBModels
{
    public interface IModeratableObject : IDbEntity
    {
        User Author { get; }
        ViewStatistic ViewStatistic { get; }
        List<Report> Reports { get; }
        List<Violation> Violations { get; }
    }
}
