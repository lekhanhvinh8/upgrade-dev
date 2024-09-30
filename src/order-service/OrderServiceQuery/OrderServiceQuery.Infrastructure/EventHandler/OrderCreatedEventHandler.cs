
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.Infrastructure.EventHandler
{
    public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
    {
        private readonly IAppUnitOfWork<WriteSide> _unitOfWork;

        public OrderCreatedEventHandler(IAppUnitOfWork<WriteSide> unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task On(OrderCreatedEvent @event)
        {
            var order = new Order()
            {
                OrderId = @event.OrderId,
                CreatedDate = DateTime.Now,
                CreatedBy = @event.CreatedBy,
                ShippingMethod = @event.ShippingMethod,
                Status = OrderStatus.Init,
            };
            await _unitOfWork.Orders.AddAsync(order);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}