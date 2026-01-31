using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Infrastructure.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Client> _dbSet;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Client>();
        }

        public async Task<Client?> FindByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Name == name);
        }

        public async Task<Client?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Client client)
        {
            await _dbSet.AddAsync(client);
        }

        public void Update(Client client)
        {
            _dbSet.Update(client);
        }
    }
}
