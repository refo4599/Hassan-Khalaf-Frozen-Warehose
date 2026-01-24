using Frozen_Warehouse.Application.DTOs.Stock;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IStockService
    {
        Task<decimal> GetStockAsync(Guid clientId, Guid productId, Guid sectionId);
    }
}