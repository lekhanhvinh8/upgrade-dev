
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using OrderServiceCommand.Core.Commands;
using OrderServiceCommand.Core.Repositories;
using OrderServiceCommand.Infrastructure.Commands;
using OrderServiceCommand.Infrastructure.ConfigurationValues;
using OrderServiceCommand.Infrastructure.Repositories;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterRepositoryExtension
    {
        public static IServiceCollection RegisterRepository(this IServiceCollection services)
        {

            // Register MongoClient as a singleton
            services.AddSingleton<IMongoClient, MongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbConfig>>().Value;
                return new MongoClient(settings.ConnectionString);
            });

            // Register IMongoDatabase as a singleton
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbConfig>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
            });

            services.AddScoped<IEventStoreRepository<ReadSide>, EventStoreRepository<ReadSide>>();
            services.AddScoped<IEventStoreRepository<WriteSide>, EventStoreRepository<WriteSide>>();
            
            return services;
        }
    }
}