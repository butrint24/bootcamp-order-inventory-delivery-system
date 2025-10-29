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
            modelBuilder.Entity<Order>(entity =>
            {
                modelBuilder.Entity<Order>(entity =>
                {
                    entity.ToTable("order"); 
                    entity.Property(e => e.OrderId).HasColumnName("order_id");
                    entity.Property(e => e.UserId).HasColumnName("user_id");
                    entity.Property(e => e.Address).HasColumnName("address");
                    entity.Property(e => e.Price).HasColumnName("price");
                    entity.Property(e => e.Status)
                        .HasConversion<string>()
                        .HasColumnName("status");                
                    entity.Property(e => e.CreatedAt)
                        .HasColumnName("created_at")
                        .HasColumnType("timestamp"); 
                    entity.Property(e => e.UpdatedAt)
                        .HasColumnName("updated_at")
                        .HasColumnType("timestamp"); 
                    entity.Property(e => e.IsDeleted).HasColumnName("is_deleted");
                });
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
