using Frozen_Warehouse.Application.DTOs.Stock;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IStockService
    {
        Task<decimal> GetStockAsync(int clientId, int productId, int sectionId);
    }
}