using Frozen_Warehouse.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Infrastructure.Data;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            // Special-case Inbound to include navigation properties (Client and Details -> Product/Section)
            if (typeof(T) == typeof(Inbound))
            {
                var query = _dbSet.AsQueryable()
                    .AsNoTracking()
                    .Include("Client")
                    .Include("Details.Product")
                    .Include("Details.Section");

                var list = await query.ToListAsync();
                return list.Cast<T>();
            }

            return await _dbSet.AsNoTracking().ToListAsync();
        }

        // Implement date range query
        public async Task<IEnumerable<T>> GetByDateRangeAsync(DateTime start, DateTime end)
        {
            if (typeof(T) == typeof(Inbound))
            {
                var inbounds = await _context.Set<Inbound>()
                    .AsNoTracking()
                    .Include(i => i.Client)
                    .Include(i => i.Details).ThenInclude(d => d.Product)
                    .Include(i => i.Details).ThenInclude(d => d.Section)
                    .Where(i => i.CreatedAt >= start && i.CreatedAt < end)
                    .ToListAsync();

                return inbounds.Cast<T>();
            }

            // Default: return all
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        // provide queryable for advanced operations (internal use only)
        public IQueryable<T> Query()
        {
            return _dbSet.AsQueryable();
        }

        // Implement daily report method required by IRepository interface
        public async Task<IEnumerable<T>> GetDailyReportAsync()
        {
            // If the repository is for Inbound, include navigation props and filter by today's CreatedAt
            if (typeof(T) == typeof(Inbound))
            {
                var inbounds = await _context.Set<Inbound>()
                    .AsNoTracking()
                    .Include(i => i.Client)
                    .Include(i => i.Details).ThenInclude(d => d.Product)
                    .Include(i => i.Details).ThenInclude(d => d.Section)
                    .Where(i => i.CreatedAt >= DateTime.UtcNow.Date)
                    .ToListAsync();

                return inbounds.Cast<T>();
            }

            // Default behavior: return all entities
            return await _dbSet.AsNoTracking().ToListAsync();
        }
    }
}