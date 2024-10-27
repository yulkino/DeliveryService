using DeliveryService.Domain;
using DeliveryService.Extensions;
using DeliveryService.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace DeliveryService.Services;

public class OrderFilterService(ApplicationContext applicationContext, ILogger<OrderFilterService> logger)
{
    public async Task<List<Order>?> GetFilteredOrders(Guid cityDistrict, DateTime firstDeliveryDateTime)
    {
        logger.LogDebug("Validating City District Id {districtId}", cityDistrict);
        var district = await applicationContext.Districts.SingleOrDefaultAsync(x => x.Id == cityDistrict);
        if (district == null)
        {
            logger.LogError("Validation fails - City District with Id {districtId} not found", cityDistrict);
            return null;
        }

        logger.LogDebug("Validation successes - Starting filtering process with " +
                              "City District Id {districtId} and " +
                              "DeliveryTime {deliveryTime}",
            district.Id, firstDeliveryDateTime);
        
        var orders = await applicationContext.Orders
            .FilterByDistrict(district)
            .FilterByDeliveryTime(firstDeliveryDateTime)
            .ToListAsync();
        
        logger.LogDebug("Filtering result contains {ordersCount} orders", orders.Count);
        return orders;
    }
}