

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OrderServiceQuery.Core.Repositories;

namespace OrderServiceQuery.Infrastructure.Repositories
{
    public class Repository<TEntity, ReposSide> : IRepository<TEntity, ReposSide> where TEntity : class where ReposSide : Side
    {
        private DbSet<TEntity> _entities;
        protected readonly DbContext Context;

        public Repository(DbContext context)
        {
            this.Context = context;
            this._entities = context.Set<TEntity>();
        }
        public async Task AddAsync(TEntity entity)
        {
            await this._entities.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await this._entities.AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            this._entities.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this._entities.RemoveRange(entities);
        }

        public async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null,
            bool isTracked = true)
        {
            var query = this._entities.Where(predicate);

            if(orderByCondition != null)
            {
                query = orderByCondition(query);
            }

            if(!isTracked)
            {
                query = query.AsNoTracking();
            }

            // This function does not use to process large sequence of data, so the limit of taking is to prevent waste of computer resource
            // Any features that need to perform large resource and having complicated processing consider to use a stored procedure
            query = query.Take(500);

            var entities = await query.ToListAsync();


            return entities;
        }

        public async Task<TEntity?> FindAsync(int id, bool isUncommited = false)
        {
            var entities = await this._entities.FindAsync(id);
            return entities;
        }

        public async Task LoadAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true)
        {
            var query = this._entities.Where(predicate);

            if(!isTracked)
            {
                query = query.AsNoTracking();
            }

            await query.LoadAsync();
        }
        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true)
        {
            TEntity? entity = null;

            if(!isTracked)
            {
                entity = await this._entities.AsNoTracking().SingleOrDefaultAsync(predicate);
            }

            entity = await this._entities.SingleOrDefaultAsync(predicate);
            return entity;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, 
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null,
            bool isTracked = true)
        {
            IQueryable<TEntity> query;

            if(!isTracked)
            {
                query = this._entities.AsNoTracking();
            }
            else
            {
                query = this._entities.AsTracking();
            }

            if(orderByCondition != null)
            {
                query = orderByCondition(query);
            }

            var entity = await query.FirstOrDefaultAsync(predicate);
            return entity;
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var countResult = await this._entities.CountAsync(predicate);
            return countResult;
        }
    }
}