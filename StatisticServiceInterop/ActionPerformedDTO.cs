using CommonDBModels;

namespace StatisticServiceInterop
{
    public class ActionPerformedDTO
    {
        public int ActionId { get; }
        public string PerformerId { get; }

        public int ActionType { get; }
    }
}
