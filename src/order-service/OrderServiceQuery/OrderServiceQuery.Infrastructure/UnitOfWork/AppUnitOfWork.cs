using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.UnitOfWork
{
    public class AppUnitOfWork<ReposSide> : UnitOfWork<ReposSide>, IAppUnitOfWork<ReposSide> where ReposSide : Side
    {
        public IOrderRepository<ReposSide> Orders { get; private set; }
        public AppUnitOfWork(OrderDbContext<ReposSide> context, 
            IOrderRepository<ReposSide> orderRepository
          )
          : base(context)
        {
            this.Orders = orderRepository;
        }
    }
}
