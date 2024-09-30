
using HealthChecks.UI.Client;
using MassTransit;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OrderServiceCommand.API.Events;
using OrderServiceCommand.Core.Producer;
using OrderServiceCommand.Infrastructure.Producers;
using OrderServiceCommand.Infrastructure.Registrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.RegisterConfigurationValue();
builder.Services.RegisterMongoDb();
builder.Services.RegisterCommands();
builder.Services.RegisterRepository();
builder.Services.RegisterEventSourcing();
builder.Services.RegsiterObservability(RegisterConfigurationValueExtension.ConnectionStrings!.Otel!);
builder.Services.RegisterLogging(new KafkaLoggingConfig() { BootstrapServers = RegisterConfigurationValueExtension.ConnectionStrings.BootstrapServers!, Topic = "log_topic" }, RegisterConfigurationValueExtension.ConnectionStrings!.Otel!);
builder.Services.AddScoped<IEventProducer, EventProducer>();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(x =>
{
    x.UsingInMemory();
    x.AddRider(rider =>
    {
        rider.AddProducer<OrderPlaced>("order-placed");
        rider.UsingKafka((context, k) =>
        {
            k.Host("localhost:9092");
        });
    });

});

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
