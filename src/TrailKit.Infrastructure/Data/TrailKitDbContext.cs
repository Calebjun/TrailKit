using Microsoft.EntityFrameworkCore;
using TrailKit.Domain.Entities.Customers;
using TrailKit.Domain.Enums;

namespace TrailKit.Infrastructure.Data;

/// <summary>
/// Author: Utsah Tamang
/// Description: EF Core context for the whole application. Holds the DbSets and the Fluent API
/// mapping. Teammates add their own DbSet lines and Fluent API configuration where marked.
/// </summary>
public class TrailKitDbContext : DbContext
{
    /// <summary>Creates the context with options supplied by dependency injection.</summary>
    /// <param name="options">EF Core options (SQL Server connection).</param>
    public TrailKitDbContext(DbContextOptions<TrailKitDbContext> options) : base(options)
    {
    }

    /// <summary>All accounts (customers, staff, managers).</summary>
    public DbSet<Customer> Customers => Set<Customer>();

    /// <summary>All charges.</summary>
    public DbSet<Charge> Charges => Set<Charge>();


    /// <summary>Configures tables, keys, indexes and relationships.</summary>
    /// <param name="modelBuilder">The EF Core model builder.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customers");
            entity.HasKey(c => c.Id);

            // One table for all customer types, told apart by a discriminator column.
            entity.HasDiscriminator<string>("CustomerType")
                  .HasValue<StandardCustomer>("Standard")
                  .HasValue<MemberCustomer>("Member");

            entity.Property(c => c.Name).IsRequired().HasMaxLength(100);
            entity.Property(c => c.Email).IsRequired().HasMaxLength(200);
            entity.HasIndex(c => c.Email).IsUnique();
            entity.Property(c => c.Phone).HasMaxLength(30);
            entity.Property(c => c.PasswordHash).IsRequired();
            entity.Property(c => c.Role).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.Standing).HasConversion<string>().HasMaxLength(20);
            entity.Property(c => c.SuspensionReason).HasMaxLength(200);

            entity.HasMany(c => c.Charges)
                  .WithOne(ch => ch.Customer)
                  .HasForeignKey(ch => ch.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Charge>(entity =>
        {
            entity.ToTable("Charges");
            entity.HasKey(ch => ch.Id);
            entity.Property(ch => ch.Amount).HasPrecision(18, 2);
            entity.Property(ch => ch.Reason).IsRequired().HasMaxLength(200);
            entity.Property(ch => ch.WaivedReason).HasMaxLength(200);
        });
    }
}
