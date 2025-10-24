namespace FullantiOAExtensions.Core.Database.Entities
{
    public class MFPOS
    {
        public string No { get; set; }
        public DateTime CreateDate { get; set; }
        public string CustomerName { get; set; }
        public string CreateUser { get; set; }
        public string Classify { get; set; }
        public string Source { get; set; }
        //public string? ModifyUser { get; set; }
        //public DateTime? ModifyTime { get; set; }
        public string? Auditor { get;set; }
    }
}
