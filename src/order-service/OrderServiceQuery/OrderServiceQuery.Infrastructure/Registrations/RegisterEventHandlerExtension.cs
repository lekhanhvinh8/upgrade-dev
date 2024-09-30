
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderServiceQuery.Core.Event;
using OrderServiceQuery.Core.EventHandler;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.EventHandler;

namespace OrderServiceQuery.Infrastructure.Registrations
{
    public static class RegisterEventHandlerExtension
    {
        public static IServiceCollection RegisterEventHandler(this IServiceCollection services)
        {
           services.AddScoped<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
           services.AddScoped<IEventHandler<OrderUpdatedEvent>, OrderUpdatedEventHandler>();

            
            return services;
        }
    }
}