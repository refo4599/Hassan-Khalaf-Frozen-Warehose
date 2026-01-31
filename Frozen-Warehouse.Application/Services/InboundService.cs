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
            // Validation
            if (string.IsNullOrWhiteSpace(request.ClientName))
                throw new ArgumentException("Client name is required");

            if (request.Lines == null || request.Lines.Count == 0)
                throw new ArgumentException("Inbound must contain at least one line");

            // Step 1: Search client by name
            var client = await _clientRepo.FindByNameAsync(request.ClientName.Trim());

            // Step 2: If not exists, create new client
            if (client == null)
            {
                client = new Client
                {
                    Name = request.ClientName.Trim()
                };
                await _clientRepo.AddAsync(client);
                await _uow.SaveChangesAsync();
            }

            // Step 3: Create inbound with client ID
            var inbound = new Inbound
            {
                ClientId = client.Id,
                CreatedAt = DateTime.UtcNow
            };

            // Process each inbound line
            foreach (var line in request.Lines)
            {
                // Validation
                if (line.Cartons < 0 || line.Pallets < 0)
                    throw new ArgumentException("Cartons and pallets must be non-negative");

                // Resolve product by name
                var product = await _productRepo.GetByNameAsync(line.ProductName.Trim());
                if (product == null) throw new ArgumentException($"Product not found: {line.ProductName}");

                // Resolve section by name
                var section = await _sectionRepo.GetByNameAsync(line.SectionName.Trim());
                if (section == null) throw new ArgumentException($"Section not found: {line.SectionName}");

                // Add inbound detail with resolved IDs
                var detail = new InboundDetail
                {
                    InboundId = inbound.Id,
                    ProductId = product.Id,
                    SectionId = section.Id,
                    Cartons = line.Cartons,
                    Pallets = line.Pallets
                };
                inbound.Details.Add(detail);

                // Update or create stock
                var stock = await _stockRepo.FindAsync(client.Id, product.Id, section.Id);
                
                if (stock == null)
                {
                    // Create new stock
                    stock = new Stock
                    {
//<<<<<<< HEAD
                        ClientId = client.Id,
                        ProductId = product.Id,
                        SectionId = section.Id,
                        Cartons = line.Cartons,
                        Pallets = line.Pallets,
                    };
                    await _stockRepo.AddAsync(stock);
                }
                else
                {
                    // Increase existing stock
                    stock.Cartons += line.Cartons;
                    stock.Pallets += line.Pallets;
                    _stockRepo.Update(stock);
                }
            }

            // Save inbound
            await _inboundRepo.AddAsync(inbound);
            await _uow.SaveChangesAsync();

            return inbound.Id;
        }
    }
}