using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface IClientRepository : IRepository<Client>
    {
        Task<Client?> FindByNameAsync(string name);
        Task<IEnumerable<Client>> GetAllAsync();
    }
}
