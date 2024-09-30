
namespace OrderServiceQuery.Core.Event
{
    public class OrderUpdatedEvent : BaseEvent
    {
        public int OrderId { get; set; }
        public int Status { get; set; }
    }
}