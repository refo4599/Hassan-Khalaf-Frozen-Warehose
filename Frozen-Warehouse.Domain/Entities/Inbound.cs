namespace Frozen_Warehouse.Domain.Entities
{
    public class Inbound
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public IList<InboundDetail> Details { get; set; } = new List<InboundDetail>();
    }
}
