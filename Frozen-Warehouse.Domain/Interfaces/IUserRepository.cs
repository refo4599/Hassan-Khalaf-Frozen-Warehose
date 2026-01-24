using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Domain.Interfaces
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> FindByUserNameAsync(string userName);
    }
}