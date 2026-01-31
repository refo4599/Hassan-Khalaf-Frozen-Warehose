using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface ISectionRepository
    {
        Task<Section?> GetByNameAsync(string name);
        Task<Section?> GetByIdAsync(int id);
        Task<IEnumerable<Section>> GetAllAsync();
    }
}
