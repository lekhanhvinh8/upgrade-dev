using Confluent.Kafka;
using MassTransit;
using ServiceInfoService.Consumer;
using ServiceInfoService.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory();
    x.AddRider(rider =>
    {
        rider.AddConsumer<OrderPlacedConsumer>();

        rider.UsingKafka((context, cfg) =>
        {
            cfg.Host("localhost:9092"); // Kafka broker address

            // Define the topic and consumer group for subscribing to OrderPlaced events
            cfg.TopicEndpoint<OrderPlaced>("order-placed", "serviceinfo-group", e =>
            {
                e.ConfigureConsumer<OrderPlacedConsumer>(context);

                // var date = new DateTime(2024, 9, 8, 12, 48, 0);
                // var utcDate = date.ToUniversalTime();
                // var offset = KafkaHelper.GetOffsetForTimestamp("order-placed", date);
                // var offsetValue = offset.Value;
                // e.Offset = offsetValue;

                //16. Kiểm tra các trường hợp nếu có sự cố lỗi kafka, thời gian reconnect --> Consume fail, reconnect in specific time
            });
        });
    });

});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();



