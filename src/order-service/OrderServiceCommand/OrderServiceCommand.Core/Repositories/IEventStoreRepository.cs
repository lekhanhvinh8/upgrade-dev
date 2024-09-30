using OrderServiceCommand.Core.Event;

namespace OrderServiceCommand.Core.Repositories
{
    public interface IEventStoreRepository<ReposSide> : IRepository<EventModel, ReposSide> where ReposSide : Side
    {
     
    }
}
