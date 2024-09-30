using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderServiceQuery.Core.Consumers;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Infrastructure.Converters;

namespace OrderServiceQuery.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IServiceProvider _serviceProvider;
        public EventConsumer(
            IOptions<ConsumerConfig> config,
            IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _serviceProvider = serviceProvider;
        }

        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                    .SetKeyDeserializer(Deserializers.Utf8)
                    .SetValueDeserializer(Deserializers.Utf8)
                    .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();

                if (consumeResult?.Message == null) continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);

                if(@event != null)
                {
                    var eventType = @event.GetType();
                    Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    var handler = _serviceProvider.GetRequiredService(handlerType);
                    var onMethod = handlerType.GetMethod("On");
                    onMethod!.Invoke(handler, new object[] { @event });
                }
                
                consumer.Commit(consumeResult);
            }
        }
    }
}
