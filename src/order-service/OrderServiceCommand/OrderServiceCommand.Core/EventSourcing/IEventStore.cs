
using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.Event;

namespace OrderServiceCommand.Core.EventSourcing
{
    public interface IEventStore<T> where T : AggregateRoot
    {
        Task SaveEventsAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion);
        Task<List<BaseEvent>> GetEventsAsync(string aggregateId);
    }
}