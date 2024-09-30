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
    }
}