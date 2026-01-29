using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Inbound
{
    public class CreateInboundRequest
    {
        public int ClientId { get; set; }
        public List<InboundLine> Lines { get; set; } = new();
    }

    public class InboundLine
    {
        public int ProductId { get; set; }
        public int SectionId { get; set; }
        public decimal Quantity { get; set; }
    }
}