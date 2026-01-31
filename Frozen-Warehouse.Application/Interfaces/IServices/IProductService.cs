using Frozen_Warehouse.Application.DTOs.Product;

namespace Frozen_Warehouse.Application.Interfaces.IServices
{
    public interface IProductService
    {
        Task<ProductDto> CreateAsync(CreateProductRequest request);
        Task<ProductDto?> GetByIdAsync(int id);
        Task<IEnumerable<ProductDto>> GetAllAsync();
    }
}
