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

        public async Task<Section?> GetByIdAsync(int id)
        {
            return await _dbSet.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Section>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().OrderBy(s => s.Name).ToListAsync();
        }

        public async Task AddAsync(Section section)
        {
            await _dbSet.AddAsync(section);
        }

        public void Update(Section section)
        {
            _dbSet.Update(section);
        }

        public void Remove(Section section)
        {
            _dbSet.Remove(section);
        }
    }
}
