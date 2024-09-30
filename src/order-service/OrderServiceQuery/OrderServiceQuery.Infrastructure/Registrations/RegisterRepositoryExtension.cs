
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.Repositories;
using OrderServiceQuery.Infrastructure.UnitOfWork;

namespace OrderServiceQuery.Infrastructure.Registrations
{
    public static class RegisterRepositoryExtension
    {
        public static IServiceCollection RegisterRepository(this IServiceCollection services)
        {
            services.AddScoped<IOrderRepository<ReadSide>, OrderRepository<ReadSide>>();
            services.AddScoped<IOrderRepository<WriteSide>, OrderRepository<WriteSide>>();

            services.AddScoped<IAppUnitOfWork<ReadSide>, AppUnitOfWork<ReadSide>>();
            services.AddScoped<IAppUnitOfWork<WriteSide>, AppUnitOfWork<WriteSide>>();
            
            return services;
        }
    }
}