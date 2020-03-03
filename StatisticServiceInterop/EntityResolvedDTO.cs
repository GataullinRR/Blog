using System;

namespace StatisticServiceInterop
{
    public class EntityResolvedDTO
    {
        public int ModeratorGroupId { get; set; }
        public string ResolverId { get; set; }

        public DateTime EntityAddTime { get; set; }
        public DateTime EntityAssignTime { get; set; }
        public DateTime EntityResolveTime { get; set; }
    }
}
