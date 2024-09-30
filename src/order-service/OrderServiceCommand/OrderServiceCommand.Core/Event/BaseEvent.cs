
using OrderServiceCommand.Core.Messages;

namespace OrderServiceCommand.Core.Event
{
    public abstract class BaseEvent : Message
    {
        protected BaseEvent()
        {
            this.Type = this.GetType().Name;
        }

        public string Type { get; set; }
        public int Version { get; set; }
    }
}