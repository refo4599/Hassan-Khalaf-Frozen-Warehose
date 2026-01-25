using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace Frozen_Warehouse.Infrastructure.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Try environment variable first
            var envConn = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
            var connectionString = envConn;

            if (string.IsNullOrEmpty(connectionString))
            {
                // Fallback to appsettings in API project if available, otherwise use default local connection
                var basePath = Directory.GetCurrentDirectory();

                // If running from infrastructure project directory, try to locate API project appsettings
                var apiPath = Path.Combine(basePath, "..", "Frozen-Warehouse.API");
                var apiSettings = Path.Combine(apiPath, "appsettings.json");

                var builder = new ConfigurationBuilder();

                if (File.Exists(apiSettings))
                {
                    builder.SetBasePath(apiPath)
                           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                           .AddEnvironmentVariables();
                }
                else
                {
                    // fallback to current directory
                    builder.SetBasePath(basePath)
                           .AddJsonFile("appsettings.json", optional: true)
                           .AddEnvironmentVariables();
                }

                var config = builder.Build();
                connectionString = config.GetConnectionString("DefaultConnection");
            }

            if (string.IsNullOrEmpty(connectionString))
            {
                // Final fallback
                connectionString = "Server=.;Database=FrozenWarehouseDb;Trusted_Connection=True;TrustServerCertificate=True;";
            }

            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}