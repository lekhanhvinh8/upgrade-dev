
using OrderServiceCommand.Core.Event;

namespace OrderServiceCommand.Core.Producer
{
    public interface IEventProducer
    {
        Task ProduceAsync<T>(string topic, T @event) where T : BaseEvent;
    }
}