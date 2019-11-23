using System.Collections.Generic;

namespace DBModels
{
    public interface IReportable : IDbEntity, IViewable, IAuthored
    {
        List<Report> Reports { get; }
        List<Violation> Violations { get; }
    }
}
