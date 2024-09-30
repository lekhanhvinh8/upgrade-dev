
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;
using OrderServiceCommand.Core.Event;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterMongoDbConfigurationExtension
    {
        public static IServiceCollection RegisterMongoDb(this IServiceCollection services)
        {
            // Register the class map for the Order entity
            BsonClassMap.RegisterClassMap<EventModel>(cm =>
            {
                cm.AutoMap(); // Automatically map properties

                // Map the Id property as BsonId
                cm.MapIdProperty(o => o.Id)
                .SetIdGenerator(StringObjectIdGenerator.Instance)  // Optional: for generating ObjectIds as strings
                .SetSerializer(new MongoDB.Bson.Serialization.Serializers.StringSerializer(MongoDB.Bson.BsonType.ObjectId)); // Store as ObjectId in MongoDB
                
                // You can also map other properties with custom configuration if needed


                BsonClassMap.RegisterClassMap<BaseEvent>();
                BsonClassMap.RegisterClassMap<OrderCreatedEvent>();
                BsonClassMap.RegisterClassMap<OrderUpdatedEvent>();
               
            });
            
            return services;
        }
    }
}