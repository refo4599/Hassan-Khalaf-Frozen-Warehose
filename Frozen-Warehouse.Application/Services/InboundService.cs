using Frozen_Warehouse.Application.DTOs.Inbound;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Domain.Entities;

namespace Frozen_Warehouse.Application.Services
{
    public class InboundService : IInboundService
    {
        private readonly IRepository<Inbound> _inboundRepo;
        private readonly IClientRepository _clientRepo;
        private readonly IProductRepository _productRepo;
        private readonly ISectionRepository _sectionRepo;
        private readonly IStockRepository _stockRepo;
        private readonly IUnitOfWork _uow;

        public InboundService(
            IRepository<Inbound> inboundRepo,
            IClientRepository clientRepo,
            IProductRepository productRepo,
            ISectionRepository sectionRepo,
            IStockRepository stockRepo,
            IUnitOfWork uow)
        {
            _inboundRepo = inboundRepo;
            _clientRepo = clientRepo;
            _productRepo = productRepo;
            _sectionRepo = sectionRepo;
            _stockRepo = stockRepo;
            _uow = uow;
        }

        public async Task<int> CreateInboundAsync(CreateInboundRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            if (string.IsNullOrWhiteSpace(request.ClientName)) throw new ArgumentException("ClientName is required");
            if (request.Lines == null || request.Lines.Count == 0) throw new ArgumentException("Inbound must contain at least one line");

            var clientName = request.ClientName.Trim();

            // find or create client
            var client = await _clientRepo.FindByNameAsync(clientName);
            if (client == null)
            {
                client = new Client { Name = clientName };
                await _clientRepo.AddAsync(client);
                await _uow.SaveChangesAsync(); // ensure client.Id is generated
            }

            // Create inbound with client ID
            var inbound = new Inbound
            {
                ClientId = client.Id,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var line in request.Lines)
            {
                if (line == null) throw new ArgumentException("Line cannot be null");
                if (string.IsNullOrWhiteSpace(line.ProductName)) throw new ArgumentException("ProductName is required");
                if (string.IsNullOrWhiteSpace(line.SectionName)) throw new ArgumentException("SectionName is required");
                if (line.Cartons < 0 || line.Pallets < 0) throw new ArgumentException("Cartons and pallets must be non-negative");

                var productName = line.ProductName.Trim();
                var sectionName = line.SectionName.Trim();

                // resolve product
                var product = await _productRepo.GetByNameAsync(productName);
                if (product == null) throw new ArgumentException($"Product not found: {productName}");

                // resolve section
                var section = await _sectionRepo.GetByNameAsync(sectionName);
                if (section == null) throw new ArgumentException($"Section not found: {sectionName}");

                // compute quantity
                var quantity = (decimal)line.Cartons + ((decimal)line.Pallets * 100m);

                var detail = new InboundDetail
                {
                    ProductId = product.Id,
                    SectionId = section.Id,
                    Cartons = line.Cartons,
                    Pallets = line.Pallets,
                    Quantity = quantity
                };

                inbound.Details.Add(detail);

                // Update or create stock
                var stock = await _stockRepo.FindAsync(client.Id, product.Id, section.Id);
                if (stock == null)
                {
                    stock = new Stock
                    {
                        ClientId = client.Id,
                        ProductId = product.Id,
                        SectionId = section.Id,
                        Cartons = line.Cartons,
                        Pallets = line.Pallets
                    };
                    await _stockRepo.AddAsync(stock);
                }
                else
                {
                    stock.Cartons += line.Cartons;
                    stock.Pallets += line.Pallets;
                    _stockRepo.Update(stock);
                }
            }

            await _inboundRepo.AddAsync(inbound);
            await _uow.SaveChangesAsync();

            return inbound.Id;
        }
    }
}