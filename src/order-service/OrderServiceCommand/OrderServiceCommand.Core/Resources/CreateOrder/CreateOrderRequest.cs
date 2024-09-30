using System.ComponentModel.DataAnnotations;

namespace OrderServiceCommand.Core.Resources.CreateOrder
{
    public class CreateOrderRequest
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string? CreatedBy { get; set; }
        [Required]
        public string? ShippingMethod { get; set; }
    }
}