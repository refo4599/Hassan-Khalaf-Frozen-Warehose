using Frozen_Warehouse.Application.DTOs.Outbound;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Application.Services
{
    public class OutboundService : IOutboundService
    {
        private readonly IRepository<Outbound> _outboundRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IUnitOfWork _uow;

        public OutboundService(IRepository<Outbound> outboundRepo, IStockRepository stockRepo, IUnitOfWork uow)
        {
            _outboundRepo = outboundRepo;
            _stockRepo = stockRepo;
            _uow = uow;
        }

        public async Task<int> CreateOutboundAsync(CreateOutboundRequest request)
        {
            if (request.Lines == null || request.Lines.Count == 0)
                throw new ArgumentException("Outbound must contain at least one line");

            var outbound = new Outbound
            {
                ClientId = request.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            // Validate all lines first to ensure atomicity
            foreach (var line in request.Lines)
            {
                if (line.Quantity <= 0) throw new ArgumentException("Quantity must be positive");
                var stock = await _stockRepo.FindAsync(request.ClientId, line.ProductId, line.SectionId);
                if (stock == null || stock.Quantity < line.Quantity)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {line.ProductId} in section {line.SectionId}");
                }
            }

            // All validations passed, perform updates
            foreach (var line in request.Lines)
            {
                var detail = new OutboundDetail
                {
                    OutboundId = outbound.Id,
                    ProductId = line.ProductId,
                    SectionId = line.SectionId,
                    Quantity = line.Quantity
                };
                outbound.Details.Add(detail);

                var stock = await _stockRepo.FindAsync(request.ClientId, line.ProductId, line.SectionId);
                stock!.Quantity -= line.Quantity;
                if (stock.Quantity < 0) throw new InvalidOperationException("Stock cannot be negative");
                _stockRepo.Update(stock);
            }

            await _outboundRepo.AddAsync(outbound);
            await _uow.SaveChangesAsync();

            return outbound.Id;
        }
    }
}