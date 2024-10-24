namespace DeliveryService.Domain;

public class Order
{
    public Guid Id { get; private set; }
    public float Weight { get; private set; }
    public District District { get; private set; }
    public DateTime DeliveryTime { get; private set; }
}