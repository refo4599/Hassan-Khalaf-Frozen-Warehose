namespace Frozen_Warehouse.Application.DTOs.Outbound
{
    public class CreateOutboundRequest
    {
        public int ClientId { get; set; }
        public List<OutboundLine> Lines { get; set; } = new();
    }

    public class OutboundLine
    {
<<<<<<< HEAD
        public Guid ProductId { get; set; }
        public Guid SectionId { get; set; }
        public int Cartons { get; set; }
        public int Pallets { get; set; }
=======
        public int ProductId { get; set; }
        public int SectionId { get; set; }
        public decimal Quantity { get; set; }
>>>>>>> 726a0b6d453ba18a5701926cf9d8477739ad96f7
    }
}