using MongoDB.Driver;
using OrderServiceCommand.Core.Event;
using OrderServiceCommand.Core.Repositories;

namespace OrderServiceCommand.Infrastructure.Repositories
{
    public class EventStoreRepository<ReposSide> : Repository<EventModel, ReposSide>, IEventStoreRepository<ReposSide> where ReposSide : Side
    {
        public EventStoreRepository(IMongoDatabase mongoDatabas)
            : base(mongoDatabas)
        {
            
        }
    }
}