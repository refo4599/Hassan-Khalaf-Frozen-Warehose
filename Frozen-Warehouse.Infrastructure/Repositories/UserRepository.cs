using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Infrastructure.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> FindByUserNameAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
        }
    }
}