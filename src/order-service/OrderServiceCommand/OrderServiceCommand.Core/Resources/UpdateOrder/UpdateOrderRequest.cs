using System.ComponentModel.DataAnnotations;

namespace OrderServiceCommand.Core.Resources.CreateOrder
{
    public class UpdateOrderRequest
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int? Status { get; set; }
      
    }
}