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

                entity.HasKey(e => e.DeliveryId);
                entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");

                
                entity.Property(e => e.Status)
                      .HasColumnName("status")
                      .HasMaxLength(20)
                      .HasDefaultValue("PENDING");

                
                entity.Property(e => e.Eta)
                      .HasColumnName("eta")
                      .HasConversion(
                          v => v, 
                          v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : null
                      );

                entity.Property(e => e.CreatedAt)
                      .HasColumnName("created_at")
                      .HasConversion(
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                      );
                entity.Property(e => e.UpdatedAt)
                      .HasColumnName("updated_at")
                      .HasConversion(
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc),
                          v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                      );

                entity.Property(e => e.IsActive)
                      .HasColumnName("is_active")
                      .HasDefaultValue(true);

                entity.Property(e => e.OrderId).HasColumnName("order_id");
                entity.Property(e => e.UserId).HasColumnName("user_id");
            });
        }
    }
}
