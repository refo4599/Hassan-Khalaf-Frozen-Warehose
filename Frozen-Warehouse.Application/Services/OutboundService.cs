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

        public async Task<Guid> CreateOutboundAsync(CreateOutboundRequest request)
        {
            if (request.Lines == null || request.Lines.Count == 0)
                throw new ArgumentException("Outbound must contain at least one line");

            var outbound = new Outbound
            {
                Id = Guid.NewGuid(),
                ClientId = request.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            // Validate all lines first to ensure atomicity
            foreach (var line in request.Lines)
            {
                if (line.Cartons < 0 || line.Pallets < 0) throw new ArgumentException("Cartons and pallets must be non-negative");
                var stock = await _stockRepo.FindAsync(request.ClientId, line.ProductId, line.SectionId);
                if (stock == null || stock.Cartons < line.Cartons || stock.Pallets < line.Pallets)
                {
                    throw new InvalidOperationException($"Insufficient stock for product {line.ProductId} in section {line.SectionId}");
                }
            }

            // All validations passed, perform updates
            foreach (var line in request.Lines)
            {
                var detail = new OutboundDetail
                {
                    Id = Guid.NewGuid(),
                    OutboundId = outbound.Id,
                    ProductId = line.ProductId,
                    SectionId = line.SectionId,
                    Cartons = line.Cartons,
                    Pallets = line.Pallets
                };
                outbound.Details.Add(detail);

                var stock = await _stockRepo.FindAsync(request.ClientId, line.ProductId, line.SectionId);
                stock!.Cartons -= line.Cartons;
                stock!.Pallets -= line.Pallets;
                if (stock.Cartons < 0 || stock.Pallets < 0) throw new InvalidOperationException("Stock cannot be negative");
                _stockRepo.Update(stock);
            }

            await _outboundRepo.AddAsync(outbound);
            await _uow.SaveChangesAsync();

            return outbound.Id;
        }
    }
}