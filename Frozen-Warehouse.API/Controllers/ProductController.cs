using Frozen_Warehouse.Application.DTOs.Product;
using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Frozen_Warehouse.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ILogger<ProductController> _logger;
        private readonly ApplicationDbContext _context;
        public ProductController(ILogger<ProductController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Select(p => new ProductResponseDto { Id = p.Id, Name = p.Name, IsActive = p.IsActive })
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = new Product
            {
                Name = dto.Name.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = null
            };

            await _context.Products.AddAsync(entity);
            await _context.SaveChangesAsync();

            var response = new ProductResponseDto { Id = entity.Id, Name = entity.Name, IsActive = entity.IsActive };

            return CreatedAtAction(nameof(GetProductById), new { id = entity.Id }, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound(new { message = "Product not found" });

            var response = new ProductResponseDto { Id = product.Id, Name = product.Name, IsActive = product.IsActive };
            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] UpdateProductDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required" });
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound(new { message = "Product not found" });

            if (!product.IsActive) return BadRequest(new { message = "Product is archived and cannot be updated" });

            var hasStock = await _context.Stocks.AnyAsync(s => s.ProductId == id);
            if (hasStock) return BadRequest(new { message = "Cannot update product that already has stock" });

            // Only allow rename via DTO mapping
            product.Name = dto.Name.Trim();
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var response = new ProductResponseDto { Id = product.Id, Name = product.Name, IsActive = product.IsActive };
            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null) return NotFound(new { message = "Product not found" });

            var hasStock = await _context.Stocks.AnyAsync(s => s.ProductId == id);
            if (hasStock) return BadRequest(new { message = "Cannot delete product because stock exists." });

            // Soft delete
            product.IsActive = false;
            product.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Product archived successfully" });
        }
        //Product Overlap تداخل اصناف
        // طرح كمية من صنف و اضافة نفس الكمية الى صنف اخر
        [HttpPost("product-overlap")]
        public async Task<IActionResult> ProductOverlap([FromBody] ProductOverlapDto dto)
        {
            if (dto == null)
                return BadRequest(new { message = "Request body is required" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (dto.SourceProductId == dto.TargetProductId)
                return BadRequest(new { message = "Source and target product must be different" });

            if (dto.Cartons < 0 || dto.Pallets < 0)
                return BadRequest(new { message = "Cartons and pallets must be non-negative" });

            if (dto.Cartons == 0 && dto.Pallets == 0)
                return BadRequest(new { message = "At least one of cartons or pallets must be greater than zero" });

            // ---------------- Resolve Client ----------------
            Client? client = null;

            if (!string.IsNullOrWhiteSpace(dto.ClientName))
            {
                var name = dto.ClientName.Trim().ToUpper();
                client = await _context.Clients.FirstOrDefaultAsync(c => c.Name.ToUpper() == name);
            }
            else if (dto.ClientId.HasValue)
            {
                client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId.Value);
            }

            if (client == null)
                return NotFound(new { message = "Client not found" });

            // ---------------- Resolve Section ----------------
            var sectionExists = await _context.Sections.AnyAsync(s => s.Id == dto.SectionId);
            if (!sectionExists)
                return NotFound(new { message = "Section not found" });

            // ---------------- Resolve Products ----------------
            var sourceProductExists = await _context.Products.AnyAsync(p => p.Id == dto.SourceProductId && p.IsActive);
            var targetProductExists = await _context.Products.AnyAsync(p => p.Id == dto.TargetProductId && p.IsActive);

            if (!sourceProductExists || !targetProductExists)
                return NotFound(new { message = "One or both products not found or inactive" });

            // ---------------- Calculate Available Stock (Inbound - Outbound) ----------------
            var inboundCartons = await (
                from d in _context.InboundDetails
                join i in _context.Inbounds on d.InboundId equals i.Id
                where d.SectionId == dto.SectionId
                      && i.ClientId == client.Id
                      && d.ProductId == dto.SourceProductId
                select (int?)d.Cartons
            ).SumAsync() ?? 0;

            var inboundPallets = await (
                from d in _context.InboundDetails
                join i in _context.Inbounds on d.InboundId equals i.Id
                where d.SectionId == dto.SectionId
                      && i.ClientId == client.Id
                      && d.ProductId == dto.SourceProductId
                select (int?)d.Pallets
            ).SumAsync() ?? 0;

            var outboundCartons = await (
                from d in _context.OutboundDetails
                join o in _context.Outbounds on d.OutboundId equals o.Id
                where d.SectionId == dto.SectionId
                      && o.ClientId == client.Id
                      && d.ProductId == dto.SourceProductId
                select (int?)d.Cartons
            ).SumAsync() ?? 0;

            var outboundPallets = await (
                from d in _context.OutboundDetails
                join o in _context.Outbounds on d.OutboundId equals o.Id
                where d.SectionId == dto.SectionId
                      && o.ClientId == client.Id
                      && d.ProductId == dto.SourceProductId
                select (int?)d.Pallets
            ).SumAsync() ?? 0;

            var availableCartons = inboundCartons - outboundCartons;
            var availablePallets = inboundPallets - outboundPallets;

            if (availableCartons < dto.Cartons || availablePallets < dto.Pallets)
                return BadRequest(new
                {
                    message = "Insufficient stock for overlap",
                    availableCartons,
                    availablePallets
                });

            // ---------------- Transaction ----------------
            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1️⃣ Create Outbound (consume from source product)
                var outbound = new Outbound
                {
                    ClientId = client.Id,
                    CreatedAt = DateTime.UtcNow,
                    Details = new List<OutboundDetail>
            {
                new OutboundDetail
                {
                    ProductId = dto.SourceProductId,
                    SectionId = dto.SectionId,
                    Cartons = dto.Cartons,
                    Pallets = dto.Pallets
                }
            }
                };

                await _context.Outbounds.AddAsync(outbound);

                // 2️⃣ Create Inbound (add to target product)
                var inbound = new Inbound
                {
                    ClientId = client.Id,
                    CreatedAt = DateTime.UtcNow,
                    Details = new List<InboundDetail>
            {
                new InboundDetail
                {
                    ProductId = dto.TargetProductId,
                    SectionId = dto.SectionId,
                    Cartons = dto.Cartons,
                    Pallets = dto.Pallets,
                    Quantity = dto.Cartons + (dto.Pallets * 100m)
                }
            }
                };

                await _context.Inbounds.AddAsync(inbound);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new
                {
                    message = "Product overlap completed successfully",
                    fromProductId = dto.SourceProductId,
                    toProductId = dto.TargetProductId,
                    cartons = dto.Cartons,
                    pallets = dto.Pallets
                });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        //Product transfer تحويل كمية نتجات وبالتات من نفس الصنف من نفس العميل من قسم الى قسم اخر
        [HttpPost("product-transfer")]
        public async Task<IActionResult> TransferProduct([FromBody] ProductTransferDto dto)
        {
            if (dto == null) return BadRequest(new { message = "Request body is required" });
            if (dto.Cartons < 0 || dto.Pallets < 0) return BadRequest(new { message = "Cartons and pallets must be non-negative" });
            if (dto.Cartons == 0 && dto.Pallets == 0) return BadRequest(new { message = "At least one of cartons or pallets must be greater than zero" });

            // validate client, product and sections
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == dto.ClientId);
            if (client == null) return NotFound(new { message = "Client not found" });

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == dto.ProductId && p.IsActive);
            if (product == null) return NotFound(new { message = "Product not found or inactive" });

            var fromSectionExists = await _context.Sections.AnyAsync(s => s.Id == dto.FromSectionId);
            var toSectionExists = await _context.Sections.AnyAsync(s => s.Id == dto.ToSectionId);
            if (!fromSectionExists || !toSectionExists) return NotFound(new { message = "One or both sections not found" });

            // compute available stock for product at client in source section (inbound - outbound)
            var inboundCartons = await (
                from d in _context.InboundDetails
                join i in _context.Inbounds on d.InboundId equals i.Id
                where d.SectionId == dto.FromSectionId && i.ClientId == dto.ClientId && d.ProductId == dto.ProductId
                select (int?)d.Cartons
            ).SumAsync() ?? 0;

            var inboundPallets = await (
                from d in _context.InboundDetails
                join i in _context.Inbounds on d.InboundId equals i.Id
                where d.SectionId == dto.FromSectionId && i.ClientId == dto.ClientId && d.ProductId == dto.ProductId
                select (int?)d.Pallets
            ).SumAsync() ?? 0;

            var outboundCartons = await (
                from d in _context.OutboundDetails
                join o in _context.Outbounds on d.OutboundId equals o.Id
                where d.SectionId == dto.FromSectionId && o.ClientId == dto.ClientId && d.ProductId == dto.ProductId
                select (int?)d.Cartons
            ).SumAsync() ?? 0;

            var outboundPallets = await (
                from d in _context.OutboundDetails
                join o in _context.Outbounds on d.OutboundId equals o.Id
                where d.SectionId == dto.FromSectionId && o.ClientId == dto.ClientId && d.ProductId == dto.ProductId
                select (int?)d.Pallets
            ).SumAsync() ?? 0;

            var availableCartons = inboundCartons - outboundCartons;
            var availablePallets = inboundPallets - outboundPallets;

            if (availableCartons < dto.Cartons || availablePallets < dto.Pallets)
                return BadRequest(new { message = "Insufficient stock in source section", availableCartons, availablePallets });

            await using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // create outbound consuming from source section
                var outbound = new Outbound
                {
                    ClientId = dto.ClientId,
                    CreatedAt = DateTime.UtcNow,
                    Details = new List<OutboundDetail>
                    {
                        new OutboundDetail
                        {
                            ProductId = dto.ProductId,
                            SectionId = dto.FromSectionId,
                            Cartons = dto.Cartons,
                            Pallets = dto.Pallets,
                            Quantity = dto.Cartons + (dto.Pallets * 100m)
                        }
                    }
                };

                await _context.Outbounds.AddAsync(outbound);

                // create inbound adding to destination section
                var inbound = new Inbound
                {
                    ClientId = dto.ClientId,
                    CreatedAt = DateTime.UtcNow,
                    Details = new List<InboundDetail>
                    {
                        new InboundDetail
                        {
                            ProductId = dto.ProductId,
                            SectionId = dto.ToSectionId,
                            Cartons = dto.Cartons,
                            Pallets = dto.Pallets,
                            Quantity = dto.Cartons + (dto.Pallets * 100m)
                        }
                    }
                };

                await _context.Inbounds.AddAsync(inbound);

                await _context.SaveChangesAsync();
                await tx.CommitAsync();

                return Ok(new { message = "Product transfer completed successfully" });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
    }
}
