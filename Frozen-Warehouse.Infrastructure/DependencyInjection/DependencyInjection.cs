using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Frozen_Warehouse.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Interfaces;
using Frozen_Warehouse.Infrastructure.Repositories;
using Frozen_Warehouse.Infrastructure.UnitOfWork;
using Frozen_Warehouse.Infrastructure.Security;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Application.Services;

namespace Frozen_Warehouse.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClientRepository, ClientRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();
            services.AddScoped<IUnitOfWork, Frozen_Warehouse.Infrastructure.UnitOfWork.UnitOfWork>();
            services.AddScoped<IStockRepository, StockRepository>();

            services.AddScoped<ITokenService, TokenService>();

            // Section service
            services.AddScoped<ISectionService, SectionService>();

            return services;
        }
    }
}