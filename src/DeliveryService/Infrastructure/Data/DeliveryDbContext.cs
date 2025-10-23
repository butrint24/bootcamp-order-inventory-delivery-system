using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace DeliveryService.Infrastructure.Data
{
    public class DeliveryDbContext : DbContext
    {
        public DeliveryDbContext(DbContextOptions<DeliveryDbContext> options)
            : base(options) { }

        public DbSet<Delivery> Deliveries => Set<Delivery>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("delivery");
                entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");
                entity.Property(e => e.Status).HasColumnName("status").HasConversion<string>();
                entity.Property(e => e.Eta).HasColumnName("eta");
                entity.Property(e => e.CreatedAt).HasColumnName("created_at");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });

        }
    }
}
