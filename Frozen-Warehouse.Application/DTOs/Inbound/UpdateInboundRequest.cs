using System.ComponentModel.DataAnnotations;

namespace Frozen_Warehouse.Application.DTOs.Inbound
{
    public class UpdateInboundRequest
    {
        [Required]
        public string ClientName { get; set; } = null!;

        [Required]
        [MinLength(1)]
        public List<InboundLineRequest> Lines { get; set; } = new();
    }
}
