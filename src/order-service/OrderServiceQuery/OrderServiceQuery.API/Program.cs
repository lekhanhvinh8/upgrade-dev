
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OrderServiceCommand.Infrastructure.Registrations;
using OrderServiceQuery.Core.Consumers;
using OrderServiceQuery.Core.Producer;
using OrderServiceQuery.Infrastructure.Consumer;
using OrderServiceQuery.Infrastructure.Consumers;
using OrderServiceQuery.Infrastructure.Producers;
using OrderServiceQuery.Infrastructure.Registrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHostedService<ConsumerHostedService>();
builder.Services.AddScoped<IEventConsumer, EventConsumer>();

builder.Services.RegisterConfigurationValue();
builder.Services.RegisterAppDbContext();
builder.Services.RegisterEventHandler();
builder.Services.RegisterRepository();
builder.Services.RegisterCaching();
builder.Services.RegsiterObservability(RegisterConfigurationValueExtension.ConnectionStrings!.Otel!);
builder.Services.RegisterLogging(new KafkaLoggingConfig() { BootstrapServers = RegisterConfigurationValueExtension.ConnectionStrings.BootstrapServers!, Topic = "log_topic" }, RegisterConfigurationValueExtension.ConnectionStrings!.Otel!);
builder.Services.AddScoped<IEventProducer, EventProducer>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// builder.Services.AddMassTransit(x =>
// {
//     x.UsingInMemory();
//     x.AddRider(rider =>
//     {
//         rider.AddProducer<OrderPlaced>("order-placed");
//         rider.UsingKafka((context, k) =>
//         {
//             k.Host("localhost:9092");
//         });
//     });

// });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseRouting();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();


app.Run();
