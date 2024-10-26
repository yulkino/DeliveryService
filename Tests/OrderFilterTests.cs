using System.Globalization;
using DeliveryService.Domain;
using DeliveryService.Infrastructure;
using DeliveryService.Services;
using Microsoft.EntityFrameworkCore;

namespace Tests;

public class OrderFilterTests
{
    private readonly ApplicationContext _context;
    private readonly OrderFilterService _orderFilterService;
    private readonly DateTime _testDateTime;
    private readonly Guid _testDistrict1Id;
    
    public OrderFilterTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        
        _context = new ApplicationContext(options);
        _context.Database.EnsureCreated();
        
        _testDateTime = new DateTime(2024, 10, 1).ToUniversalTime();
        _testDistrict1Id = Guid.NewGuid();
        
        Seed();
        _context.SaveChanges();
        
        _orderFilterService = new OrderFilterService(_context);
    }
    
    [Fact]
    public async Task OrderFilterTests_ShouldReturnCorrectOrders_WhenValidFilter()
    {
        var orders = await _orderFilterService.GetFilteredOrders(_testDistrict1Id, _testDateTime);
        
        Assert.NotNull(orders);
        
        var districtIds = orders
            .Select(o => o.District.Id)
            .ToList();
        
        Assert.Equal(2, orders.Count);
        Assert.Contains(_testDistrict1Id, districtIds);
    }
    
    [Fact]
    public async Task OrderFilterTests_ShouldReturnNull_WhenInvalidFilter()
    {
        var orders = await _orderFilterService.GetFilteredOrders(Guid.NewGuid(), _testDateTime);
        
        Assert.Null(orders);
    }
    
    private void Seed()
    {
        var districts = SeedDistricts();
        SeedOrders(districts);
    }

    private List<District> SeedDistricts()
    {
        var districts = new List<District>
        {
            new()
            {
                Id = _testDistrict1Id,
                Name = "Test District1",
                CityName = "Test City1",
            }
        };
        _context.Districts.AddRange(districts);
        return districts;
    }

    private void SeedOrders(List<District> districts)
    {
        var ordersWithDifferentIntervals = new List<Order>
        {
            new()
            {
                Id = Guid.NewGuid(),
                Weight = float.Parse(new Random().NextDouble().ToString(CultureInfo.InvariantCulture)),
                DeliveryTime = _testDateTime.AddMinutes(5),
                District = districts.First(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Weight = float.Parse(new Random().NextDouble().ToString(CultureInfo.InvariantCulture)),
                DeliveryTime = _testDateTime.AddMinutes(10),
                District = districts.First(),
            },
            new()
            {
                Id = Guid.NewGuid(),
                Weight = float.Parse(new Random().NextDouble().ToString(CultureInfo.InvariantCulture)),
                DeliveryTime = _testDateTime.AddMinutes(35),
                District = districts.First(),
            }
        };

        _context.Orders.AddRange(ordersWithDifferentIntervals);
    }
    
    public void Dispose()
    {
        _context.Dispose();
    }
}