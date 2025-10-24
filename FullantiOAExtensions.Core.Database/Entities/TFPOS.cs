namespace FullantiOAExtensions.Core.Database.Entities
{
    public class TFPOS
    {
        public string ProductNo { get; set; }
        public string ProductName { get; set; }
        public string Specification { get; set; }
        public decimal Quantity {  get; set; }
        public decimal Price { get; set; }
        public decimal? ReceivePrice { get; set; }
        public string? Remark { get; set; }
        public decimal Total { get { return this.Price * this.Quantity; } }
    }
}
