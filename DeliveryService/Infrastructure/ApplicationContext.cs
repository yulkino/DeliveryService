using DeliveryService.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryService.Infrastructure;

public sealed class ApplicationContext(DbContextOptions<ApplicationContext> options) : DbContext(options)
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<District> Districts { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Order>(ConfigureOrderEntity);
        modelBuilder.Entity<District>(ConfigureDistrictEntity);
    }

    private static void ConfigureOrderEntity(EntityTypeBuilder<Order> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.DeliveryTime)
            .IsRequired();
        
        entity.Property(e => e.Weight)
            .IsRequired();

        entity.HasOne(e => e.District)
            .WithMany()
            .HasForeignKey("DistrictId")
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureDistrictEntity(EntityTypeBuilder<District> entity)
    {
        entity.HasKey(e => e.Id);

        entity.Property(e => e.Name)
            .IsRequired();
        
        entity.Property(e => e.CityName)
            .IsRequired();
    }
}