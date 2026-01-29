using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface IStockRepository
    {
        Task<Stock?> FindAsync(int clientId, int productId, int sectionId);
        Task AddAsync(Stock stock);
        void Update(Stock stock);
    }
}