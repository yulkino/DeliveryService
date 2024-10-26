using DeliveryService.Domain;
using DeliveryService.Extensions;
using DeliveryService.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


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
        [FromServices] ApplicationContext applicationContext) => 
        await applicationContext.Orders
            .Include(x => x.District)
            .ToListAsync())
    .WithName("GetOrders")
    .WithOpenApi();

app.MapGet("/districts", async (
        [FromServices] ApplicationContext applicationContext) =>
        await applicationContext.Districts.ToListAsync())
    .WithName("GetDistricts")
    .WithOpenApi();

app.MapGet("/orders/filter/{cityDistrict:guid}", async (
        [FromRoute] Guid cityDistrict,
        [FromQuery] DateTime firstDeliveryDateTime,
        [FromServices] ApplicationContext applicationContext) => 
        await GetFilteredOrders(cityDistrict, firstDeliveryDateTime, applicationContext))
    .WithName("FilterOrders")
    .WithOpenApi();

app.Run();
return;

async Task<Results<Ok<List<Order>>, NotFound<string>>> GetFilteredOrders(
    Guid cityDistrict, 
    DateTime firstDeliveryDateTime, 
    ApplicationContext applicationContext)
{
    var district = await applicationContext.Districts.SingleOrDefaultAsync(x => x.Id == cityDistrict);
    if (district == null)
    {
        return TypedResults.NotFound($"District with ID {cityDistrict} not found");
    }
        
    var orders = await applicationContext.Orders
        .FilterByDistrict(district)
        .FilterByDeliveryTime(firstDeliveryDateTime)
        .ToListAsync();
        
    return TypedResults.Ok(orders);
}