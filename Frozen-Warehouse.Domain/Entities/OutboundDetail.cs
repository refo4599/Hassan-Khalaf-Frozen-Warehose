using System.ComponentModel.DataAnnotations.Schema;

namespace Frozen_Warehouse.Domain.Entities
{
    public class OutboundDetail
    {
        public int Id { get; set; }
        public  int OutboundId { get; set; }
        public Outbound Outbound { get; set; } = null!;

        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int SectionId { get; set; }
        public Section Section { get; set; } = null!;

<<<<<<< HEAD
        public int Cartons { get; set; }
        public int Pallets { get; set; }
=======
        [Column(TypeName = "decimal(18,2)")]
        public decimal Quantity { get; set; }
>>>>>>> 726a0b6d453ba18a5701926cf9d8477739ad96f7
    }
}
