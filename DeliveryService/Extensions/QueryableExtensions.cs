using DeliveryService.Domain;

namespace DeliveryService.Extensions;

public static class QueryableExtensions
{
    public static IQueryable<Order> FilterByDeliveryTime(this IQueryable<Order> orders, DateTime deliveryTime) =>
        orders.Where(o => 
            o.DeliveryTime >= deliveryTime && 
            o.DeliveryTime <= deliveryTime.AddMinutes(30));
    
    public static IQueryable<Order> FilterByDistrict(this IQueryable<Order> orders, District district) =>
        orders.Where(o => o.District == district);
}