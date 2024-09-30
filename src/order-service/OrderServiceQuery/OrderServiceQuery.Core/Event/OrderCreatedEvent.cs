
namespace OrderServiceQuery.Core.Event
{
    public class OrderCreatedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public string? CreatedBy { get; set; }
        public string? ShippingMethod { get; set; }
    }
}