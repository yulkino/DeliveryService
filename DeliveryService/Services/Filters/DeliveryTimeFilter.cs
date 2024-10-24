using DeliveryService.Domain;

namespace DeliveryService.Services.Filters;

public class DeliveryTimeFilter(DateTime deliveryTime) : IOrderFilter
{
    public IQueryable<Order> Filter(IQueryable<Order> orders) =>
        orders.Where(o => 
            o.DeliveryTime >= deliveryTime && 
            o.DeliveryTime <= deliveryTime.AddMinutes(30));
}