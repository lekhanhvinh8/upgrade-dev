
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;


        public OrderCreatedEventHandler(IAppUnitOfWork<WriteSide> unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
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

            var cacheOptions = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
            };
            await _cache.SetStringAsync("OrderServiceQuery_Order_OrderId_" + order.OrderId.ToString(), JsonSerializer.Serialize(order), cacheOptions);
        }
    }
}