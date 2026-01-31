using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Infrastructure.Repositories
{
    public class SectionRepository : ISectionRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<Section> _dbSet;

        public SectionRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<Section>();
        }

        public async Task<Section?> GetByNameAsync(string name)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Name == name);
        }

        public async Task<Section?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
        }
    }
}
