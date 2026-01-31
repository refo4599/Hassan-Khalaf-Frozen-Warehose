namespace Frozen_Warehouse.Domain.Entities
{
    public class Outbound
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        // Navigation
        public IList<OutboundDetail> Details { get; set; } = new List<OutboundDetail>();
    }
}
