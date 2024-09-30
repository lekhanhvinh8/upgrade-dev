using OrderServiceQuery.Core.Resources.CreateOrder;

namespace OrderServiceQuery.Core.Commands
{
    public class CreateOrderCommand : BaseCommand
    {
        public CreateOrderRequest Request { get; set; }

        public CreateOrderCommand(CreateOrderRequest request)
        {
            Request = request;
        }
    }
}
