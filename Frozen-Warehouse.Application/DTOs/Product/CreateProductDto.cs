using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Product
{
    public class CreateProductDto
    {
        [Required]
        [MaxLength(250)]
        public string Name { get; set; } = null!;
    }
}
