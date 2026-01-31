using Frozen_Warehouse.Application.DTOs.Stock;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IStockService
    {
        Task<StockResponse> GetStockAsync(int clientId, int productId, int sectionId);
        Task<decimal> GetStockQuantityAsync(int clientId, int productId, int sectionId);
    }
}