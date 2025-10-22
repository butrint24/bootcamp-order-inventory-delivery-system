using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace OrderService.Infrastructure.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options) { }

        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_item");
                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.ProductId).HasColumnName("product_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("order_item");
                entity.Property(e => e.OrderItemId).HasColumnName("order_item_id");
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.Quantity).HasColumnName("quantity");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
            });
        }
    }
}
