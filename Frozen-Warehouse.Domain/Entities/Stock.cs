using System.ComponentModel.DataAnnotations.Schema;

namespace Frozen_Warehouse.Domain.Entities
{
    public class Stock
    {
        public int Id { get; set; }

        public int ClientId { get; set; }
        public Client Client { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
    }
}
