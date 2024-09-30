
using OrderServiceQuery.Core.Event;

namespace OrderServiceQuery.Core.EventHandler
{
    public interface IEventHandler<TBaseEvent> where TBaseEvent : BaseEvent
    {
        public Task On(TBaseEvent @event);
    }
}