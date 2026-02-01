using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Client
{
    public class CreateClientRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = null!;
    }
}
