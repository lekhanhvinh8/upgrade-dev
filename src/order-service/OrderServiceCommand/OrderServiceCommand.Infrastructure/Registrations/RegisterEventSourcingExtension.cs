
using Microsoft.Extensions.DependencyInjection;
using OrderServiceCommand.Core.Domain;
using OrderServiceCommand.Core.EventSourcing;
using OrderServiceCommand.Infrastructure.EventSourcing;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterEventSourcingExtension
    {
        public static IServiceCollection RegisterEventSourcing(this IServiceCollection services)
        {

            services.AddScoped<IEventSourcingHandler<OrderAggregate>, OrderEventSourcingHandler>();
            services.AddScoped<IEventStore<OrderAggregate>, EventStore<OrderAggregate>>();
            
            return services;
        }
    }
}