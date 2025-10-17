using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace InventoryService.Infrastructure.Data
{
    public class ProductDbContext : DbContext
    {
        public ProductDbContext(DbContextOptions<ProductDbContext> options)
            : base(options) { }

        public DbSet<Product> Products => Set<Product>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("product");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Name).HasColumnName("name");
                entity.Property(e => e.Stock).HasColumnName("stock");
                entity.Property(e => e.Origin).HasColumnName("origin");
                entity.Property(e => e.Price).HasColumnName("price");
                entity.Property(e => e.Category).HasColumnName("category").HasConversion<string>();
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);

            });

        }
    }
}
