namespace Frozen_Warehouse.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation
        public ICollection<InboundDetail> InboundDetails { get; set; } = new List<InboundDetail>();
        public ICollection<OutboundDetail> OutboundDetails { get; set; } = new List<OutboundDetail>();
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
