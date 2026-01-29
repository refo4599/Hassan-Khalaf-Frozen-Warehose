using Frozen_Warehouse.Application.DTOs.Inbound;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Application.Services
{
    public class InboundService : IInboundService
    {
        private readonly IRepository<Inbound> _inboundRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IUnitOfWork _uow;

        public InboundService(IRepository<Inbound> inboundRepo, IStockRepository stockRepo, IUnitOfWork uow)
        {
            _inboundRepo = inboundRepo;
            _stockRepo = stockRepo;
            _uow = uow;
        }

        public async Task<int> CreateInboundAsync(CreateInboundRequest request)
        {
            if (request.Lines == null || request.Lines.Count == 0)
                throw new ArgumentException("Inbound must contain at least one line");

            var inbound = new Inbound
            {
                ClientId = request.ClientId,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var line in request.Lines)
            {
                if (line.Quantity <= 0) throw new ArgumentException("Quantity must be positive");

                var detail = new InboundDetail
                {
                    InboundId = inbound.Id,
                    ProductId = line.ProductId,
                    SectionId = line.SectionId,
                    Quantity = line.Quantity
                };
                inbound.Details.Add(detail);

                var stock = await _stockRepo.FindAsync(request.ClientId, line.ProductId, line.SectionId);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ClientId = request.ClientId,
                        ProductId = line.ProductId,
                        SectionId = line.SectionId,
                        Quantity = line.Quantity
                    };
                    await _stockRepo.AddAsync(stock);
                }
                else
                {
                    stock.Quantity += line.Quantity;
                    _stockRepo.Update(stock);
                }
            }

            await _inboundRepo.AddAsync(inbound);
            await _uow.SaveChangesAsync();

            return inbound.Id;
        }
    }
}