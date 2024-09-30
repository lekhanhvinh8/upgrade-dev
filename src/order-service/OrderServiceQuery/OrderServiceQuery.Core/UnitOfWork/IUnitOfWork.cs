

using System.Data;
using OrderServiceQuery.Core.Repositories;

namespace OrderServiceQuery.Infrastructure.UnitOfWork
{
    public interface IUnitOfWork<ReposSide> : IDisposable where ReposSide : Side
    {
        public Task<IEnumerable<T>> Query<T>(string query, CommandType commandType = CommandType.Text, object? parameters = null, string? connectionString = null, IDictionary<string, object?>? outputParameters = null);
        void Attach<TEntity>(TEntity entity);
        void AttachRange(IEnumerable<object> entities);
        void Detach<TEntity>(TEntity entity);
        public void ClearTracking();
        public Task<int> SaveChangesAsync();
    }
}
