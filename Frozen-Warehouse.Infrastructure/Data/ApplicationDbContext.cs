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

            // seed users with fixed GUIDs and plain text passwords (testing only)
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