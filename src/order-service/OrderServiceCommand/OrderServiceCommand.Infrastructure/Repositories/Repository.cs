

using System.Linq.Expressions;
using MongoDB.Driver;
using OrderServiceCommand.Core.Repositories;

namespace OrderServiceCommand.Infrastructure.Repositories
{
    public class Repository<TEntity, ReposSide> : IRepository<TEntity, ReposSide> where TEntity : class where ReposSide : Side
    {
        private readonly IMongoCollection<TEntity> _eventStoreCollection;

        public Repository(IMongoDatabase mongoDatabase)
        {
            //Connection Pooling: MongoDB MongoClient is designed to be reused. 
            // It automatically handles connection pooling, so creating a new MongoClient for every request is inefficient and can lead to resource exhaustion.
            var typeOfTEntity = typeof(TEntity);
            var entityName = typeOfTEntity.Name;
            _eventStoreCollection = mongoDatabase.GetCollection<TEntity>(entityName);
        }
        public async Task AddAsync(TEntity entity)
        {
            await _eventStoreCollection.InsertOneAsync(entity).ConfigureAwait(false);
        }

        public async Task<IEnumerable<TEntity>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            var result = await _eventStoreCollection.Find(predicate).ToListAsync();
            return result;
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate)
        {
            var entity = await _eventStoreCollection.Find(predicate).FirstOrDefaultAsync();
            return entity;
        }
     
    }
}