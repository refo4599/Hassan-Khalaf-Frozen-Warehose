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
    }
}
