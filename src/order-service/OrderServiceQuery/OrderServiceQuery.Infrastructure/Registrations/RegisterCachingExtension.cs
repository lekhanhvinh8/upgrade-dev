using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.Registrations
{
    public static class RegisterCachingExtension
    {
        public static IServiceCollection RegisterCaching(this IServiceCollection services)
        {
            try
            {
                services.AddDbContext<OrderDbContext<ReadSide>>(options => options.UseSqlServer(RegisterConfigurationValueExtension.ConnectionStrings!.SqlServer!));
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = RegisterConfigurationValueExtension.ConnectionStrings!.RedisCache!;
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: " + ex.Message + ", StackTrace: " + ex.StackTrace);
            }

            return services;
        }
    }
}