using System.Net;
using System.Linq.Expressions;

namespace OrderServiceCommand.Core.Repositories
{
    public interface IRepository<TEntity, ReposSide> where TEntity : class where ReposSide : Side
    {
        Task AddAsync(TEntity entity);
        Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
