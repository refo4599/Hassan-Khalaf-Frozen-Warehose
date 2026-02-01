using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface ISectionRepository : IRepository<Section>
    {
        Task<Section?> GetByNameAsync(string name);
        Task<IEnumerable<Section>> GetAllAsync();
    }
}
