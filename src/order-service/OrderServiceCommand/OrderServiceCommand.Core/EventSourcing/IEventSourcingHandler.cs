using OrderServiceCommand.Core.Domain;

namespace OrderServiceCommand.Core.EventSourcing
{
    public interface IEventSourcingHandler<T>
    {
        Task SaveAsync(AggregateRoot aggregate);
        Task<T> GetByIdAsync(string aggregateId);
    }
}