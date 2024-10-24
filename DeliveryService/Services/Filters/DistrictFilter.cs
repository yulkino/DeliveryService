using DeliveryService.Domain;

namespace DeliveryService.Services.Filters;

public class DistrictFilter(District district) : IOrderFilter
{
    public IQueryable<Order> Filter(IQueryable<Order> orders) =>
        orders.Where(o => o.District == district);
}