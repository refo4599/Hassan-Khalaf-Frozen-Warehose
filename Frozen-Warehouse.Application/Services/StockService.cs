using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Application.Services
{
    public class StockService : IStockService
    {
        private readonly IStockRepository _stockRepo;

        public StockService(IStockRepository stockRepo)
        {
            _stockRepo = stockRepo;
        }

        public async Task<decimal> GetStockAsync(int clientId, int productId, int sectionId)
        {
            var stock = await _stockRepo.FindAsync(clientId, productId, sectionId);
            return stock?.Quantity ?? 0m;
        }
    }
}