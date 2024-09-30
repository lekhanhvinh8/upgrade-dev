
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
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
        private readonly IDistributedCache _cache;

        public OrderUpdatedEventHandler(IAppUnitOfWork<WriteSide> unitOfWork, IDistributedCache cache)
        {
            _unitOfWork = unitOfWork;
            _cache = cache;
        }

        public async Task On(OrderUpdatedEvent @event)
        {
            var order = await _unitOfWork.Orders.FirstOrDefaultAsync(o => o.OrderId == @event.OrderId);
            if(order != null)
            {
                order.Status = @event.Status;
                await _unitOfWork.SaveChangesAsync();

                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };
                await _cache.SetStringAsync("OrderServiceQuery_Order_OrderId_" + order.OrderId.ToString(), JsonSerializer.Serialize(order), cacheOptions);
            }
        }
    }
}