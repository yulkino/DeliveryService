using DeliveryService.Domain;

namespace DeliveryService.Services.Filters;

public interface IOrderFilter
{
    IQueryable<Order> Filter(IQueryable<Order> orders);
}