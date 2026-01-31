using Microsoft.EntityFrameworkCore;
using Frozen_Warehouse.Domain.Entities;
using Frozen_Warehouse.Domain.Enums;

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

            // Seed 8 sections
            var sections = new[]
            {
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000001"), Name = "S1" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000002"), Name = "S2" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000003"), Name = "S3" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000004"), Name = "S4" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000005"), Name = "S5" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000006"), Name = "S6" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000007"), Name = "S7" },
                new Section { Id = Guid.Parse("10000000-0000-0000-0000-000000000008"), Name = "S8" }
            };
            modelBuilder.Entity<Section>().HasData(sections);

            // Seed users with fixed GUIDs and plain text passwords (testing only)
            var adminId = Guid.Parse("00000000-0000-0000-0000-000000000001");
            var skId = Guid.Parse("00000000-0000-0000-0000-000000000002");

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminId, Username = "admin", Password = "123456", Role = Roles.Admin },
                new User { Id = skId, Username = "storekeeper", Password = "123456", Role = Roles.StoreKeeper }
            );
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return base.SaveChangesAsync(cancellationToken);
        }
    }
}