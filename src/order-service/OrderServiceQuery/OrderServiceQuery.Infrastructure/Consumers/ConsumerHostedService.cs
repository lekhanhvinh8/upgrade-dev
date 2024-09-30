using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderServiceQuery.Core.Consumers;

namespace OrderServiceQuery.Infrastructure.Consumer
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostedService(ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event Consumer Service running.");

            Task.Run(async () => {
                using (IServiceScope scope = _serviceProvider.CreateScope())
                {
                    var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                    await eventConsumer.Consume("OrderEventSourcing");
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Event Consumer Service Stopped");

            return Task.CompletedTask;
        }
    }
}