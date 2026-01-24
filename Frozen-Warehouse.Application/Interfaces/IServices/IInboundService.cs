using Frozen_Warehouse.Application.DTOs.Inbound;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IInboundService
    {
        Task<Guid> CreateInboundAsync(CreateInboundRequest request);
    }
}