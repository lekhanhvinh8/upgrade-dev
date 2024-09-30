

using System.Linq.Expressions;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using OrderServiceQuery.Core.Repositories;

namespace OrderServiceQuery.Infrastructure.Repositories
{
    public class CacheRepository<TEntity, ReposSide> : IRepository<TEntity, ReposSide> 
        where TEntity : class where ReposSide : Side
    {
        protected readonly IRepository<TEntity, ReposSide> _repository;
        protected readonly IDistributedCache _cache;
        private readonly string _cacheKeyPrefix = "OrderServiceQuery_" + typeof(TEntity).Name + "_Id_"; 

        public CacheRepository(DbContext context,
            IRepository<TEntity, ReposSide> repository,
            IDistributedCache cache)
        {

            this._repository = repository;
            _cache = cache;
        }

        private string GetEntityId(TEntity entity)
        {
            return entity.GetType().GetProperty("Id")!.GetValue(entity)!.ToString()!;
        }

        public async Task AddAsync(TEntity entity)
        {
            await this._repository.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await this._repository.AddRangeAsync(entities);
        }

        public void Remove(TEntity entity)
        {
            this._repository.Remove(entity);
        }

        public void RemoveRange(IEnumerable<TEntity> entities)
        {
            this._repository.RemoveRange(entities);
        }

        public async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null, bool isTracked = true)
        {
            return await this._repository.Where(predicate, orderByCondition, isTracked);
        }

        public async Task<TEntity?> FindAsync(int id, bool isUncommited = false)
        {
            var cacheValue = await _cache.GetStringAsync(_cacheKeyPrefix + id);
            if(!string.IsNullOrEmpty(cacheValue))
            {
                var entityObj = JsonSerializer.Deserialize<TEntity>(cacheValue);
                return entityObj;
            }

            var entity = await this._repository.FindAsync(id, isUncommited);

            if(entity != null)
            {
                var cacheOptions = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
                };

                await _cache.SetStringAsync(_cacheKeyPrefix + GetEntityId(entity).ToString(), JsonSerializer.Serialize(entity), cacheOptions);

            }
            return entity;
        }

        public async Task LoadAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true)
        {
            await this._repository.LoadAsync(predicate, isTracked);
        }

        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracked = true)
        {
            return await this._repository.SingleOrDefaultAsync(predicate, isTracked);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByCondition = null, bool isTracked = true)
        {
            return await this._repository.FirstOrDefaultAsync(predicate, orderByCondition, isTracked);
        }

        public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return await this._repository.CountAsync(predicate);
        }
    }
}