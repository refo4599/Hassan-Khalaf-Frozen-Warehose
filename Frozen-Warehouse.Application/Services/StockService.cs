using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Application.DTOs.Stock;
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

        public async Task<StockResponse> GetStockAsync(Guid clientId, Guid productId, Guid sectionId)
        {
            var stock = await _stockRepo.FindAsync(clientId, productId, sectionId);
            if (stock == null) return new StockResponse { ClientId = clientId, ProductId = productId, SectionId = sectionId, Cartons = 0, Pallets = 0 };
            return new StockResponse { ClientId = stock.ClientId, ProductId = stock.ProductId, SectionId = stock.SectionId, Cartons = stock.Cartons, Pallets = stock.Pallets };
        }
    }
}