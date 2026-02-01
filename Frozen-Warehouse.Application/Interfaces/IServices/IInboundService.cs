using Frozen_Warehouse.Application.DTOs.Inbound;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IInboundService
    {
        Task<int> CreateInboundAsync(CreateInboundRequest request);
        Task<IEnumerable<Inbound>> GetAllInboundsAsync();
    }
}