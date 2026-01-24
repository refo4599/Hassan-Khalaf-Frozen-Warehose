namespace Frozen_Warehouse.Application.DTOs.Outbound
{
    public class CreateOutboundRequest
    {
        public Guid ClientId { get; set; }
        public List<OutboundLine> Lines { get; set; } = new();
    }

    public class OutboundLine
    {
        public Guid ProductId { get; set; }
        public Guid SectionId { get; set; }
        public decimal Quantity { get; set; }
    }
}