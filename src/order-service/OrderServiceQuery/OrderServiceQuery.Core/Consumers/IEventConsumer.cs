namespace OrderServiceQuery.Core.Consumers
{
    public interface IEventConsumer
    {
        void Consume(string topic);
    }
}