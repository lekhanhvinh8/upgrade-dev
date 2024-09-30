using OrderServiceQuery.Core.Repositories;

namespace OrderServiceQuery.Infrastructure.UnitOfWork
{
    public interface IAppUnitOfWork<ReposSide> : IUnitOfWork<ReposSide> where ReposSide : Side
    {
        IOrderRepository<ReposSide> Orders { get; }
    }
}
