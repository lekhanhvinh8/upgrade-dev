using OrderServiceQuery.Core.Resources.CreateOrder;

namespace OrderServiceQuery.Core.Commands
{
    public class UpdateOrderCommand : BaseCommand
    {
        public UpdateOrderRequest Request { get; set; }

        public UpdateOrderCommand(UpdateOrderRequest request)
        {
            Request = request;
        }
    }
}
