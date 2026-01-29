using System.ComponentModel.DataAnnotations.Schema;

namespace Frozen_Warehouse.Domain.Entities
{
    public class Stock
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public Guid ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public Guid SectionId { get; set; }
        public Section Section { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
    }
}
