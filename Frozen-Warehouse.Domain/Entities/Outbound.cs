namespace Frozen_Warehouse.Domain.Entities
{
    public class Outbound
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public IList<OutboundDetail> Details { get; set; } = new List<OutboundDetail>();
    }
}