using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.Event;
using OrderServiceCommand.Core.EventSourcing;
using OrderServiceCommand.Core.Producer;
using OrderServiceCommand.Core.Repositories;


namespace OrderServiceCommand.Infrastructure.EventSourcing
{
    public class EventStore<AggregateType> : IEventStore<AggregateType> where AggregateType : AggregateRoot
    {
        private readonly IEventStoreRepository<ReadSide> _readEventStoreRepository;
        private readonly IEventStoreRepository<WriteSide> _writeEventStoreRepository;

        private readonly IEventProducer _eventProducer;

        public EventStore(
            IEventStoreRepository<ReadSide> readEventStoreRepository, 
            IEventStoreRepository<WriteSide> writeEventStoreRepository,
            IEventProducer eventProducer
        )
        {
            _eventProducer = eventProducer;
            _readEventStoreRepository = readEventStoreRepository;
            _writeEventStoreRepository = writeEventStoreRepository;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId)
        {
            var aggregateType = typeof(AggregateType);
            var eventStream = await _readEventStoreRepository.Where(e => e.AggregateIdentifier == aggregateId && e.AggregateType == aggregateType.Name);

            if (eventStream == null || !eventStream!.Any())
                throw new Exception("Incorrect post ID provided!");

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList()!;
        }

        public async Task SaveEventsAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
        {
            var aggregateType = typeof(AggregateType);

            var result = await _writeEventStoreRepository.Where(e => e.AggregateIdentifier == aggregateId && e.AggregateType == aggregateType.Name);
            var eventStream = result.ToList();
            
            if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
                throw new Exception("Invalid version");

            var version = expectedVersion;

            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    TimeStamp = DateTime.Now,
                    AggregateIdentifier = aggregateId,
                    AggregateType = aggregateType.Name,
                    Version = version,
                    EventType = eventType,
                    EventData = @event
                };

                await _writeEventStoreRepository.AddAsync(eventModel);

                await _eventProducer.ProduceAsync("OrderEventSourcing", @event);
            }
        }
    }
}