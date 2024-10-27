using DeliveryService.Domain;
using DeliveryService.Infrastructure;
using DeliveryService.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<OrderFilterService>();

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File(
        path: "logs/log-.txt",
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7, 
        fileSizeLimitBytes: 10_000_000,
        rollOnFileSizeLimit: true,
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level}] {Message}{NewLine}{Exception}"
    )
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

await using var scope = app.Services.CreateAsyncScope();
var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
await context.Database.MigrateAsync();

app.MapGet("/orders", async (
        [FromServices] ApplicationContext applicationContext,
        [FromServices] ILogger<Program> logger) =>
    {
        logger.LogInformation("Getting orders...");
        
        var orders =  await applicationContext.Orders
            .Include(x => x.District)
            .ToListAsync();
        
        logger.LogInformation("Got {ordersCount} orders", orders.Count);
        return orders;
    })
    .WithName("GetOrders")
    .WithOpenApi();

app.MapGet("/districts", async (
        [FromServices] ApplicationContext applicationContext,
        [FromServices] ILogger<Program> logger) =>
    {
        logger.LogInformation("Getting districts...");
        
        var districts = await applicationContext.Districts.ToListAsync();
        
        logger.LogInformation("Got {ordersCount} orders", districts.Count);
        return districts;
    })
    .WithName("GetDistricts")
    .WithOpenApi();

app.MapGet("/orders/filter/{cityDistrict:guid}", async (
        [FromRoute] Guid cityDistrict,
        [FromQuery] DateTime firstDeliveryDateTime,
        [FromServices] OrderFilterService orderFilterService,
        [FromServices] ILogger<Program> logger) => 
        await GetFilteredOrders(cityDistrict, firstDeliveryDateTime, orderFilterService, logger))
    .WithName("FilterOrders")
    .WithOpenApi();

app.Run();
return;

async Task<Results<Ok<List<Order>>, NotFound<string>>> GetFilteredOrders(
    Guid cityDistrict, 
    DateTime firstDeliveryDateTime, 
    OrderFilterService orderFilterService,
    ILogger<Program> logger)
{
    logger.LogInformation("Getting orders with filters: City District Id {districtId}, DeliveryTime {deliveryTime}", 
        cityDistrict, firstDeliveryDateTime);
    
    var result = await orderFilterService.GetFilteredOrders(cityDistrict, firstDeliveryDateTime);
    if (result == null)
    {
        logger.LogError("Validation error caused by District with Id {districtId}", cityDistrict);
        return TypedResults.NotFound($"District with ID {cityDistrict} not found");
    }
    
    logger.LogInformation("Got {ordersCount} orders", result.Count);
    return TypedResults.Ok(result);
}