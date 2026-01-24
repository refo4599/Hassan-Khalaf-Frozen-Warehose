using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Infrastructure.Data;
using Frozen_Warehouse.Infrastructure.Repositories;
using Frozen_Warehouse.Application.Interfaces.IServices;
using Frozen_Warehouse.Application.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Frozen_Warehouse.Domain.Interfaces;
using System.Diagnostics;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Frozen_Warehouse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Repositories
            builder.Services.AddScoped(typeof(Frozen_Warehouse.Domain.Interfaces.IRepository<>), typeof(Repository<>));
            builder.Services.AddScoped<Frozen_Warehouse.Domain.Interfaces.IUserRepository, UserRepository>();

            // Services
            builder.Services.AddScoped<IAuthService, AuthService>();

            // JWT
            var jwtKey = builder.Configuration["Jwt:Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("Configuration value 'Jwt:Key' is missing. Set it in appsettings.json, appsettings.Development.json, user-secrets, or environment variables.");
            }

            var key = Encoding.UTF8.GetBytes(jwtKey);
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            // Always enable Swagger and set it to the application root so opening the root URL shows Swagger UI.
            app.MapOpenApi();

            // Ensure the Swagger JSON is served
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                // Serve the UI at application root
                c.RoutePrefix = string.Empty;

                // Explicitly point Swagger UI to the generated OpenAPI/Swagger JSON.
                // Swashbuckle generates the document at "/swagger/v1/swagger.json" by default.
                // Some OpenAPI helpers may expose it at "/v1/swagger.json"; add the Swashbuckle path so the UI can load it.
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Frozen Warehouse API v1");
            });

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Launch default browser to the first listening URL when the app has started.
            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                try
                {
                    // Prefer the first configured URL from app.Urls, fall back to common defaults.
                    var url = app.Urls.FirstOrDefault();
                    if (string.IsNullOrEmpty(url))
                    {
                        // Common Kestrel defaults used in development logs
                        url = builder.Environment.IsDevelopment() ? "https://localhost:7006" : "http://localhost:5000";
                    }

                    // Ensure swagger root path
                    if (!url.EndsWith('/')) url += '/';

                    var psi = new ProcessStartInfo
                    {
                        FileName = url,
                        UseShellExecute = true
                    };
                    Process.Start(psi);
                }
                catch
                {
                    // ignore any errors when attempting to start the browser
                }
            });

            app.Run();
        }
    }
}
