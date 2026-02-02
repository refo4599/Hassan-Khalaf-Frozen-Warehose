using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Domain.Enums;
using System;

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

            modelBuilder.Entity<Client>().HasIndex(c => c.Name).IsUnique();

            modelBuilder.Entity<Product>().HasQueryFilter(p => p.IsActive);

            modelBuilder.Entity<Stock>().HasIndex(s => new { s.ClientId, s.ProductId, s.SectionId }).IsUnique();

            // Configure relationships from Stock side to avoid requiring navigation collections on Client/Product/Section
            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Client)
                .WithMany()
                .HasForeignKey(s => s.ClientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Stock>()
                .HasOne(s => s.Product)
                .WithMany(p => p.Stocks)
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

            // Seed 8 sections
            var sections = new[]
            {
                new Section { Id = 1, Name = "S1" },
                new Section { Id = 2, Name = "S2" },
                new Section { Id = 3, Name = "S3" },
                new Section { Id = 4, Name = "S4" },
                new Section { Id = 5, Name = "S5" },
                new Section { Id = 6, Name = "S6" },
                new Section { Id = 7, Name = "S7" },
                new Section { Id = 8, Name = "S8" }
            };
            modelBuilder.Entity<Section>().HasData(sections);

            // Seed users with integer ids and plain text passwords (testing only)
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "admin", Password = "123456", Role = Roles.Admin },
                new User { Id = 2, Username = "storekeeper", Password = "123456", Role = Roles.StoreKeeper }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}