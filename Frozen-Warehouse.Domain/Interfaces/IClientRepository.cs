using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface IClientRepository
    {
        Task<Client?> FindByNameAsync(string name);
        Task<Client?> GetByIdAsync(Guid id);
        Task AddAsync(Client client);
        void Update(Client client);
    }
}
