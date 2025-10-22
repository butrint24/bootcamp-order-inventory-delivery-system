using Microsoft.EntityFrameworkCore;
using Shared.Entities;

namespace UserService.Infrastructure.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Tel).IsUnique();
                entity.HasIndex(e => e.Email).IsUnique();

                entity.Property(e => e.UserId).HasColumnName("user_id");
                entity.Property(e => e.Name).HasColumnName("name").HasMaxLength(50);
                entity.Property(e => e.Surname).HasColumnName("surname").HasMaxLength(50);
                entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
                entity.Property(e => e.Tel).HasColumnName("tel").HasMaxLength(50);
                entity.Property(e => e.Email).HasColumnName("email").HasMaxLength(100);
                entity.Property(e => e.Address).HasColumnName("address");
                entity.Property(e => e.IsActive).HasColumnName("is_active").HasDefaultValue(true);
                entity.Property(e => e.Role)
                    .HasColumnName("role")
                    .HasConversion<string>()
                    .HasMaxLength(20);
            });
        }

    }
}
