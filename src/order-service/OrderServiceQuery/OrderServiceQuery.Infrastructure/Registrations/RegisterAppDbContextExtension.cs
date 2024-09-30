using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderServiceQuery.Core.Repositories;
using OrderServiceQuery.Infrastructure.DatabaseContext;

namespace OrderServiceQuery.Infrastructure.Registrations
{
    public static class RegisterAppDbContextExtension
    {
        public static IServiceCollection RegisterAppDbContext(this IServiceCollection services)
        {
            try
            {
                services.AddDbContext<OrderDbContext<ReadSide>>(options => options.UseSqlServer(RegisterConfigurationValueExtension.ConnectionStrings!.SqlServer!));
                services.AddDbContext<OrderDbContext<WriteSide>>(options => options.UseSqlServer(RegisterConfigurationValueExtension.ConnectionStrings!.SqlServer!));

                services.AddDbContext<MigrationOrderDbContext>(options => options.UseSqlServer(RegisterConfigurationValueExtension.ConnectionStrings!.SqlServer!));

            }
            catch (Exception ex)
            {
                Console.WriteLine("ex: " + ex.Message + ", StackTrace: " + ex.StackTrace);
            }

            return services;
        }
    }
}