using System.Net;
using System.Linq.Expressions;

namespace OrderServiceQuery.Core.Repositories
{
    public interface IRepository<TEntity, ReposSide> where TEntity : class where ReposSide : Side
    {
        Task AddAsync(TEntity entity);
        Task AddRangeAsync(IEnumerable<TEntity> entities);
        void Remove(TEntity entity);
        void RemoveRange(IEnumerable<TEntity> entities);
        Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null,
            bool isTracked = true);
        Task<TEntity?> FindAsync(int id, bool isUncommited = false);
        Task LoadAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null,
            bool isTracked = true);
        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);
    }
}
