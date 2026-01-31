using Frozen_Warehouse.Application.DTOs.Stock;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IStockService
    {
<<<<<<< HEAD
        Task<StockResponse> GetStockAsync(Guid clientId, Guid productId, Guid sectionId);
=======
        Task<decimal> GetStockAsync(int clientId, int productId, int sectionId);
>>>>>>> 726a0b6d453ba18a5701926cf9d8477739ad96f7
    }
}