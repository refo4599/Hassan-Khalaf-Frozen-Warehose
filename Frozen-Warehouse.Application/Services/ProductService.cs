using Frozen_Warehouse.Application.DTOs.Product;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Domain.Interfaces;

namespace Frozen_Warehouse.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IRepository<Product> _repo;
        private readonly IUnitOfWork _uow;

        public ProductService(IRepository<Product> repo, IUnitOfWork uow)
        {
            _repo = repo;
            _uow = uow;
        }

        public async Task<ProductDto> CreateAsync(CreateProductRequest request)
        {
            var product = new Product { Name = request.Name };
            await _repo.AddAsync(product);
            await _uow.SaveChangesAsync();
            return new ProductDto { Id = product.Id, Name = product.Name };
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            // Use repository Query via casting to implementation if available
            // Fall back to GetById for empty list scenario is not ideal; instead query DBSet if repository supports it
            var repoImpl = _repo as dynamic;
            try
            {
                var query = repoImpl.Query() as IQueryable<Product>;
                var list = await System.Threading.Tasks.Task.FromResult(query.Select(p => new ProductDto { Id = p.Id, Name = p.Name }));
                return list;
            }
            catch
            {
                return Enumerable.Empty<ProductDto>();
            }
        }

        public async Task<ProductDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;
            return new ProductDto { Id = entity.Id, Name = entity.Name };
        }
    }
}
