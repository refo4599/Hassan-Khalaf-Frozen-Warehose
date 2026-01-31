namespace Frozen_Warehouse.Domain.Entities
{
    public class OutboundDetail
    {
        public Guid Id { get; set; }
        public Guid OutboundId { get; set; }
        public Outbound Outbound { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        public int Cartons { get; set; }
        public int Pallets { get; set; }
    }
}