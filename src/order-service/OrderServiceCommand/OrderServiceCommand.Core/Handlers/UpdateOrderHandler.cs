
using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.EventSourcing;

namespace OrderServiceCommand.Core.Commands
{
    public class UpdateOrderHandler : ICommandHandler<UpdateOrderCommand, int>
    {
        private readonly IEventSourcingHandler<OrderAggregate> _orderEventSourcingHandler;

        public UpdateOrderHandler(IEventSourcingHandler<OrderAggregate> orderEventSourcingHandler)
        {
            _orderEventSourcingHandler = orderEventSourcingHandler;
        }

        public async Task<int> Handle(UpdateOrderCommand command, CancellationToken cancellation)
        {
            var request = command.Request;
            var aggregate = await _orderEventSourcingHandler.GetByIdAsync(request.OrderId.ToString());
            aggregate.Update((int) request.OrderId, (int)request.Status!);

            await _orderEventSourcingHandler.SaveAsync(aggregate);
            return 1;
        }
    }
}



