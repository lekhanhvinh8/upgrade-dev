using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.EventSourcing;


namespace OrderServiceCommand.Infrastructure.EventSourcing
{
    public class OrderEventSourcingHandler : IEventSourcingHandler<OrderAggregate>
    {
        private readonly IEventStore<OrderAggregate> _eventStore;

        public OrderEventSourcingHandler(IEventStore<OrderAggregate> eventStore)
        {
            _eventStore = eventStore;
        }

        public async Task<OrderAggregate> GetByIdAsync(string aggregateId)
        {
            var aggregate = new OrderAggregate();
            var events = await _eventStore.GetEventsAsync(aggregateId);

            if (events == null || !events.Any()) return aggregate;

            aggregate.ReplayEvents(events);
            aggregate.Version = events.Select(x => x.Version).Max();

            return aggregate;
        }

        public async Task SaveAsync(AggregateRoot aggregate)
        {
            await _eventStore.SaveEventsAsync(aggregate.AggregateId, aggregate.GetUncommittedChanges(), aggregate.Version);
            aggregate.MarkChangesAsCommitted();
        }
    }
}