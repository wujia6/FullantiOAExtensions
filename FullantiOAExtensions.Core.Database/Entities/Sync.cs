namespace FullantiOAExtensions.Core.Database.Entities
{
    public class Sync
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Source { get; set; }
        public string Classify { get; set; }
        public int State { get; set; }
        public string Operator { get; set; }
        public string? Auditor { get; set; }
        public DateTime? AuditTime { get; set; }
    }
}
