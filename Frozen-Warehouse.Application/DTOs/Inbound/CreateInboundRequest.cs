using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Inbound
{
    public class CreateInboundRequest
    {
        public Guid ClientId { get; set; }
        public List<InboundLine> Lines { get; set; } = new();
    }

    public class InboundLine
    {
        public Guid ProductId { get; set; }
        public Guid SectionId { get; set; }
        public decimal Quantity { get; set; }
    }
}