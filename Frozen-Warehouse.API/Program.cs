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
using Microsoft.OpenApi.Models;
using Frozen_Warehouse.Infrastructure.DependencyInjection;

namespace Frozen_Warehouse.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var configuration = builder.Configuration;

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // CORS - allow Angular frontend
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAngular", policy =>
                {
                    policy.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            // Authorization services kept but role enforcement disabled in middleware for development
            builder.Services.AddAuthorization();

            // Swagger with JWT support
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Frozen Warehouse API", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
            });

            // Infrastructure registration (DbContext, repositories, token service)
            builder.Services.AddInfrastructure(configuration);

            // Application services
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IInboundService, InboundService>();
            builder.Services.AddScoped<IOutboundService, OutboundService>();
            builder.Services.AddScoped<IStockService, StockService>();

            // JWT
            var jwtSection = configuration.GetSection("Jwt");
            var jwtKey = jwtSection["Key"];
            if (string.IsNullOrWhiteSpace(jwtKey))
            {
                throw new InvalidOperationException("Configuration value 'Jwt:Key' is missing. Set it in appsettings.json or environment variables.");
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
                    ValidateIssuer = true,
                    ValidIssuer = jwtSection["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSection["Audience"],
                    ClockSkew = TimeSpan.Zero
                };
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.RoutePrefix = string.Empty;
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Frozen Warehouse API v1");
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("AllowAngular");

            app.UseAuthentication();

            // TODO: Re-enable authorization after development
            // app.UseAuthorization();

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
