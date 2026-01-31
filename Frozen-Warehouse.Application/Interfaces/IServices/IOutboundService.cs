using Frozen_Warehouse.Application.DTOs.Outbound;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IOutboundService
    {
        Task<int> CreateOutboundAsync(CreateOutboundRequest request);
    }
}