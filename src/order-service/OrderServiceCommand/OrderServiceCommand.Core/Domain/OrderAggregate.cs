


using OrderServiceCommand.Core.Event;

namespace OrderServiceCommand.Core.Domain
{
    public class OrderAggregate : AggregateRoot
    {
        public int? OrderId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ShippingMethod { get; set; }
        public int? Status { get; set; }

        public List<OrderItem> Items { get; set; }

        public OrderAggregate()
        {
            Items = new();
        }

        public OrderAggregate(int orderId, string createdBy, string shipingMethod)
        {
            Items = new();
            RaiseEvent(new OrderCreatedEvent() { OrderId = orderId, CreatedBy = createdBy, ShippingMethod = shipingMethod });
        }

        public void Apply(OrderCreatedEvent @event)
        {
            AggregateId = @event.OrderId.ToString();
            OrderId = @event.OrderId;
            CreatedBy = @event.CreatedBy;
            ShippingMethod = @event.ShippingMethod;

            Items = new();
            Status = OrderStatus.Init;
            CreatedDate = DateTime.Now;
        }

        public void Update(int orderId, int status)
        {
            RaiseEvent(new OrderUpdatedEvent() { OrderId = orderId, Status = status });
        }

        public void Apply(OrderUpdatedEvent @event)
        {
           Status = @event.Status;
        }
    }

    public class OrderStatus
    {
        public const int Init = 0;
        public const int Confirm = 1;
        public const int Shipping = 2;
        public const int Shipped = 3;
    }
}