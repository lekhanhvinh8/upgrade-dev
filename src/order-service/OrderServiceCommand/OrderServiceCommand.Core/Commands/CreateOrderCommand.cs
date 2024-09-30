using OrderServiceCommand.Core.Resources.CreateOrder;

namespace OrderServiceCommand.Core.Commands
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
