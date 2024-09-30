
namespace OrderServiceCommand.Core.Domain
{
    public class OrderItem
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public string? CreatedBy { get; set; }
        public string? Name { get; set; }
        public double Price { get; set; }
        public OrderAggregate? Order { get; set; }
    }
}

