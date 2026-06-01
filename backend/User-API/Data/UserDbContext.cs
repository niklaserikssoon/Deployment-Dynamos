using Microsoft.EntityFrameworkCore;
using User_API.Models;

namespace User_API.Data;

public class UserDbContext : DbContext
{
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__1788CC4C95CEE1D3");

            entity.HasIndex(e => e.Email)
                  .IsUnique()
                  .HasDatabaseName("UQ__Users__A9D105341D46A09F");

            entity.HasIndex(e => e.UserName)
                  .IsUnique()
                  .HasDatabaseName("UQ__Users__C9F284565A30D118");

            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.UserName).HasMaxLength(100);
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        base.OnModelCreating(modelBuilder);
    }
}