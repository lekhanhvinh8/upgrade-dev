

namespace OrderServiceQuery.Core.Domain
{
    public class Order
    {
        public int Id { get; set; }
        public int? OrderId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? ShippingMethod { get; set; }
        public int? Status { get; set; }

        //public List<OrderItem> Items { get; set; }
    }

    public class OrderStatus
    {
        public const int Init = 0;
        public const int Confirm = 1;
        public const int Shipping = 2;
        public const int Shipped = 3;
    }
}