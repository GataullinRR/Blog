using System.Collections.Generic;

namespace DBModels
{
    public interface IReportObject : IDbEntity
    {
        List<Report> Reports { get; }
    }
}
