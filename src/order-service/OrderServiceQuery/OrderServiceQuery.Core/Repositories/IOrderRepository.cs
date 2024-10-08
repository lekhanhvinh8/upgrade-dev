


using OrderServiceQuery.Core.Domain;

namespace OrderServiceQuery.Core.Repositories
{
    public interface IOrderRepository<ReposSide> : IRepository<Order, ReposSide> where ReposSide : Side
    {
        public Task<Order?> GetOrderByOrderIdAsync(int orderId);
    }
}