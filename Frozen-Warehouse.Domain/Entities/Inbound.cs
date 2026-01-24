namespace Frozen_Warehouse.Domain.Entities
{
    public class Inbound
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public IList<InboundDetail> Details { get; set; } = new List<InboundDetail>();
    }
}