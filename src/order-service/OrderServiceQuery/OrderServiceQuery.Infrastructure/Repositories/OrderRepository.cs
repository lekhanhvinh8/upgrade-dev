using Microsoft.EntityFrameworkCore;
using OrderServiceQuery.Core.Domain;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.Repositories
{
    public class OrderRepository<ReposSide> : Repository<Order, ReposSide>, IOrderRepository<ReposSide> where ReposSide : Side
    {
        private readonly OrderDbContext<ReposSide> _context;

        public OrderRepository(OrderDbContext<ReposSide> context)
            : base(context)
        {
            _context = context;
        }

        public async Task<Order?> GetOrderByOrderIdAsync(int orderId)
        {
            return await _context.Orders.FirstOrDefaultAsync(x => x.OrderId == orderId);
        }
    }
}