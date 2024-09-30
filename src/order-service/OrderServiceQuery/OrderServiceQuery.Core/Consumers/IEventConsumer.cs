namespace OrderServiceQuery.Core.Consumers
{
    public interface IEventConsumer
    {
        Task Consume(string topic);
    }
}