using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;
using System.Linq;

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

        public async Task<Client?> GetByIdAsync(int id)
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

        public void Remove(Client client)
        {
            _dbSet.Remove(client);
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetDailyReportAsync()
        {
            // Return clients created today
            return await _dbSet.AsNoTracking()
                .Where(c => c.CreatedAt >= DateTime.UtcNow.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Client>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            return await _dbSet.AsNoTracking()
                .Where(c => c.CreatedAt >= start && c.CreatedAt < end)
                .ToListAsync();
        }
    }
}
