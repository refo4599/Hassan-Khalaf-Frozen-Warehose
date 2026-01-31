using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Infrastructure.Repositories
{
    public class StockRepository : IStockRepository
    {
        private readonly ApplicationDbContext _context;

        public StockRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Stock?> FindAsync(int clientId, int productId, int sectionId)
        {
            return await _context.Stocks.FirstOrDefaultAsync(s => s.ClientId == clientId && s.ProductId == productId && s.SectionId == sectionId);
        }

        public async Task AddAsync(Stock stock)
        {
            await _context.Stocks.AddAsync(stock);
        }

        public void Update(Stock stock)
        {
            _context.Stocks.Update(stock);
        }
    }
}