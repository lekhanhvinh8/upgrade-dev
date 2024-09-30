
using Microsoft.Extensions.DependencyInjection;
using OrderServiceCommand.Core.Commands;
using OrderServiceCommand.Infrastructure.Commands;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterCommandsExtension
    {
        public static IServiceCollection RegisterCommands(this IServiceCollection services)
        {
            services.AddScoped<ICommandDispatcher, CommandDispatcher>();
            
            services.AddScoped<ICommandHandler<CreateOrderCommand, int>, CreateOrderHandler>();
            services.AddScoped<ICommandHandler<UpdateOrderCommand, int>, UpdateOrderHandler>();

            
            return services;
        }
    }
}