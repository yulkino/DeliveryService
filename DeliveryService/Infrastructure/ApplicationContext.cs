using Bogus;
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
        
        SeedRandomData(modelBuilder);
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

    private static void SeedRandomData(ModelBuilder modelBuilder)
    {
        var districtCount = 3;
        var districtIds = Enumerable
            .Range(0, districtCount)
            .Select(_ => Guid.NewGuid()).
            ToList();

        var districtFaker = new Faker<District>()
            .RuleFor(d => d.Id, f => districtIds[f.IndexFaker])
            .RuleFor(d => d.Name, f => f.Address.CityPrefix() + " District")
            .RuleFor(d => d.CityName, f => f.Address.City());

        var districts = districtFaker.Generate(districtCount);
        modelBuilder
            .Entity<District>()
            .HasData(districts);
        
        var orderFaker = new Faker<Order>()
            .RuleFor(o => o.Id, _ => Guid.NewGuid())
            .RuleFor(o => o.Weight, f => f.Random.Float(0.5f, 100))
            .RuleFor(o => o.DeliveryTime, f => f.Date.Soon().ToUniversalTime())
            .RuleFor(o => o.District, f => districts[f.Random.Int(0, districts.Count - 1)]);

        var orders = orderFaker.Generate(1000);
        modelBuilder
            .Entity<Order>()
            .HasData(orders.Select(order => new
            {
                order.Id,
                order.Weight,
                DistrictId = order.District.Id,
                order.DeliveryTime
            }));
    }
}