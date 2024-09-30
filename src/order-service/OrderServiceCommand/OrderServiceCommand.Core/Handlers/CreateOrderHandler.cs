
using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.EventSourcing;

namespace OrderServiceCommand.Core.Commands
{
    public class CreateOrderHandler : ICommandHandler<CreateOrderCommand, int>
    {
        private readonly IEventSourcingHandler<OrderAggregate> _orderEventSourcingHandler;

        public CreateOrderHandler(IEventSourcingHandler<OrderAggregate> orderEventSourcingHandler)
        {
            _orderEventSourcingHandler = orderEventSourcingHandler;
        }

        public async Task<int> Handle(CreateOrderCommand command, CancellationToken cancellation)
        {
            var request = command.Request;
            var aggregate = new OrderAggregate(request.OrderId, request.CreatedBy!, request.ShippingMethod!);

            await _orderEventSourcingHandler.SaveAsync(aggregate);

            return 1;
        }
    }
}



