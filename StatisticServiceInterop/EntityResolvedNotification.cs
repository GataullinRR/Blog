using System;

namespace StatisticServiceExports
{
    [Serializable]
    public class EntityResolvedNotification
    {
        public int ModeratorGroupId { get; set; }
        public string ResolverId { get; set; }

        public DateTime EntityAddTime { get; set; }
        public DateTime EntityAssignTime { get; set; }
        public DateTime EntityResolveTime { get; set; }

        public EntityResolvedNotification(int moderatorGroupId, string resolverId, DateTime entityAddTime, DateTime entityAssignTime, DateTime entityResolveTime)
        {
            ModeratorGroupId = moderatorGroupId;
            ResolverId = resolverId ?? throw new ArgumentNullException(nameof(resolverId));
            EntityAddTime = entityAddTime;
            EntityAssignTime = entityAssignTime;
            EntityResolveTime = entityResolveTime;
        }
    }
}
