namespace Frozen_Warehouse.Domain.Entities
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        // Navigation
        public ICollection<Inbound> Inbounds { get; set; } = new List<Inbound>();
        public ICollection<Outbound> Outbounds { get; set; } = new List<Outbound>();
        public ICollection<Stock> Stocks { get; set; } = new List<Stock>();
    }
}
