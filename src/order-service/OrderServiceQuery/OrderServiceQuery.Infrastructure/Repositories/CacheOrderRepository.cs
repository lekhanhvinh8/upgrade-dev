using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.Repositories
{
    public class CacheOrderRepository : CacheRepository<Order, ReadSide>, IOrderRepository<ReadSide> 
    {
        private readonly OrderDbContext<ReadSide> _context;
        private readonly IOrderRepository<ReadSide> _repository;
        private const string _prefix = "OrderServiceQuery_Order_OrderId_";

        public CacheOrderRepository(OrderDbContext<ReadSide> context, IOrderRepository<ReadSide> repository, IDistributedCache cache)
            : base(context, repository, cache)
        {
            _context = context;
            _repository = repository;
        }

        public async Task<Order?> GetOrderByOrderIdAsync(int orderId)
        {
            var cacheValue = await _cache.GetStringAsync(_prefix + orderId.ToString());
            if(!string.IsNullOrEmpty(cacheValue))
            {
                var cacheOrderObj = JsonSerializer.Deserialize<Order>(cacheValue);
                return cacheOrderObj;
            }

            var order = await _repository.GetOrderByOrderIdAsync(orderId);

            if(order != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };
                await _cache.SetStringAsync(_prefix + orderId.ToString(), JsonSerializer.Serialize(order), cacheOptions);
            }

            return order;
        }
    }
}