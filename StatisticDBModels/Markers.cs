namespace StatisticDBModels
{
    /// <summary>
    /// Are used to create different tables for the same entity (workaround)
    /// </summary>
    public static class Markers
    {
        public sealed class AClass { AClass() { } }
        public sealed class BClass { BClass() { } }
        public sealed class CClass { CClass() { } }
    }
}
