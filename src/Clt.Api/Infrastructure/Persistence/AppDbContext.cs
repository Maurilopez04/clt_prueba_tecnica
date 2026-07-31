using Clt.Api.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Clt.Api.Infrastructure.Persistence;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Address> Addresses => Set<Address>();
    public DbSet<Currency> Currencies => Set<Currency>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Name).HasMaxLength(120).IsRequired();
            entity.Property(user => user.Email).HasMaxLength(254).IsRequired();
            entity.Property(user => user.PasswordHash).HasMaxLength(256);
            entity.Property(user => user.IsActive).HasDefaultValue(true);
            entity.HasIndex(user => user.Email).IsUnique();
        });

        modelBuilder.Entity<Address>(entity =>
        {
            entity.ToTable("Addresses");
            entity.HasKey(address => address.Id);
            entity.Property(address => address.Street).HasMaxLength(200).IsRequired();
            entity.Property(address => address.City).HasMaxLength(100).IsRequired();
            entity.Property(address => address.Country).HasMaxLength(100).IsRequired();
            entity.Property(address => address.ZipCode).HasMaxLength(20);
            entity.HasOne(address => address.User)
                .WithMany(user => user.Addresses)
                .HasForeignKey(address => address.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currencies");
            entity.HasKey(currency => currency.Id);
            entity.Property(currency => currency.Code).HasMaxLength(3).IsRequired();
            entity.Property(currency => currency.Name).HasMaxLength(80).IsRequired();
            entity.Property(currency => currency.RateToBase).HasPrecision(18, 6).IsRequired();
            entity.HasIndex(currency => currency.Code).IsUnique();
        });
    }
}
