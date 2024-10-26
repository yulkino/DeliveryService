namespace DeliveryService.Domain;

public class Order
{
    public Guid Id { get; set; }
    public float Weight { get; set; }
    public District District { get; set; }
    public DateTime DeliveryTime { get; set; }
}