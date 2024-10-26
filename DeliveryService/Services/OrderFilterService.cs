using DeliveryService.Domain;
using DeliveryService.Extensions;
using DeliveryService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Services;

public class OrderFilterService(ApplicationContext applicationContext)
{
    public async Task<List<Order>?> GetFilteredOrders(Guid cityDistrict, DateTime firstDeliveryDateTime)
    {
        var district = await applicationContext.Districts.SingleOrDefaultAsync(x => x.Id == cityDistrict);
        if (district == null)
        {
            return null;
        }
        
        var orders = await applicationContext.Orders
            .FilterByDistrict(district)
            .FilterByDeliveryTime(firstDeliveryDateTime)
            .ToListAsync();
        
        return orders;
    }
}