using System.ComponentModel.DataAnnotations;

namespace OrderServiceQuery.Core.Resources.CreateOrder
{
    public class UpdateOrderRequest
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public int? Status { get; set; }
      
    }
}