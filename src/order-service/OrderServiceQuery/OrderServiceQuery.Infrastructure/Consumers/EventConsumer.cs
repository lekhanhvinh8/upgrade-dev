using System.Diagnostics;
using System.Text.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry;
using OpenTelemetry.Trace;
using OrderServiceQuery.Core.Consumers;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Infrastructure.Converters;
using Serilog;

namespace OrderServiceQuery.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IServiceProvider _serviceProvider;
        private readonly TracerProvider _tracerProvider;
        public EventConsumer(
            IOptions<ConsumerConfig> config,
            IServiceProvider serviceProvider,
            TracerProvider tracerProvider)
        {
            _config = config.Value;
            _serviceProvider = serviceProvider;
            _tracerProvider = tracerProvider;
        }

        public async Task Consume(string topic)
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

                var activitySource = new ActivitySource("ConsumeProcess");
                using (var myActivity = activitySource.StartActivity("HandleMessage"))
                {
                    Log.Information($"Consumed message '{consumeResult.Message.Value}' at: '{consumeResult.TopicPartitionOffset}'.");

                    var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                    var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);

                    if(@event != null)
                    {
                        var eventType = @event.GetType();
                        Type handlerType = typeof(IEventHandler<>).MakeGenericType(eventType);
                        var handler = _serviceProvider.GetRequiredService(handlerType);
                        var onMethod = handlerType.GetMethod("On");
                        var task = (Task?)onMethod!.Invoke(handler, new object[] { @event });
                        if(task != null)
                        {
                            await task;
                        }
                    }
                    
                    consumer.Commit(consumeResult);
                }
            }
        }
    }
}
