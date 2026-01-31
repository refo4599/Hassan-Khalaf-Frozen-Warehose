using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Inbound
{
    public class CreateInboundRequest
    {
<<<<<<< HEAD
        [Required]
        public string ClientName { get; set; } = null!;

        [Required]
        [MinLength(1)]
=======
        public int ClientId { get; set; }
>>>>>>> 726a0b6d453ba18a5701926cf9d8477739ad96f7
        public List<InboundLine> Lines { get; set; } = new();
    }

    public class InboundLine
    {
<<<<<<< HEAD
        [Required]
        public string ProductName { get; set; } = null!;

        [Required]
        public string SectionName { get; set; } = null!;

        [Range(0, int.MaxValue)]
        public int Cartons { get; set; }

        [Range(0, int.MaxValue)]
        public int Pallets { get; set; }
=======
        public int ProductId { get; set; }
        public int SectionId { get; set; }
        public decimal Quantity { get; set; }
>>>>>>> 726a0b6d453ba18a5701926cf9d8477739ad96f7
    }
}