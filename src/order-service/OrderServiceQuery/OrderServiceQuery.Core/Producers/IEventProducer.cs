
using OrderServiceQuery.Core.Event;

namespace OrderServiceQuery.Core.Producer
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
    }
}