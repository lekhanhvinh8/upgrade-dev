
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.Infrastructure.EventHandler
{
    public class OrderUpdatedEventHandler : IEventHandler<OrderUpdatedEvent>
    {
        private readonly IAppUnitOfWork<WriteSide> _unitOfWork;

        public OrderUpdatedEventHandler(IAppUnitOfWork<WriteSide> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task On(OrderUpdatedEvent @event)
        {
            var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderId == @event.OrderId);
            if(order != null)
            {
                order.Status = @event.Status;
            }
            await _unitOfWork.SaveChangesAsync();
        }
    }
}