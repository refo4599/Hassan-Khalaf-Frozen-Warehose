using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Domain.Enums;
using System.Text;
using System.Security.Cryptography;

namespace Frozen_Warehouse.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Section> Sections { get; set; } = null!;
        public DbSet<Stock> Stocks { get; set; } = null!;
        public DbSet<Inbound> Inbounds { get; set; } = null!;
        public DbSet<InboundDetail> InboundDetails { get; set; } = null!;
        public DbSet<Outbound> Outbounds { get; set; } = null!;
        public DbSet<OutboundDetail> OutboundDetails { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Stock>().HasIndex(s => new { s.ClientId, s.ProductId, s.SectionId }).IsUnique();

            modelBuilder.Entity<Stock>().Property(s => s.Quantity).HasPrecision(18, 2);
            modelBuilder.Entity<InboundDetail>().Property(d => d.Quantity).HasPrecision(18, 2);
            modelBuilder.Entity<OutboundDetail>().Property(d => d.Quantity).HasPrecision(18, 2);

            // Configure relationships from Stock side to avoid requiring navigation collections on Client/Product/Section
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany()
                .HasForeignKey(s => s.ProductId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Section)
                .WithMany()
                .HasForeignKey(s => s.SectionId)
                .OnDelete(DeleteBehavior.Restrict);

            // relationships for inbound/outbound details
            modelBuilder.Entity<InboundDetail>().HasOne(d => d.Inbound).WithMany(i => i.Details).HasForeignKey(d => d.InboundId).OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<OutboundDetail>().HasOne(d => d.Outbound).WithMany(i => i.Details).HasForeignKey(d => d.OutboundId).OnDelete(DeleteBehavior.Cascade);

            // seed minimal data
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var skId = Guid.Parse("00000000-0000-0000-0000-000000000002");

            // use a simple SHA256 hash for seed (in prod use a proper password hasher)
            static string Hash(string plain)
            {
                using var sha = SHA256.Create();
                var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(plain));
                return Convert.ToBase64String(bytes);
            }

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminId, UserName = "admin", PasswordHash = Hash("password"), Role = Roles.Admin },
                new User { Id = skId, UserName = "storekeeper", PasswordHash = Hash("password"), Role = Roles.StoreKeeper }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}