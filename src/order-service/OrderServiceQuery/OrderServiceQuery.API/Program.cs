
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using OrderServiceCommand.Infrastructure.Registrations;
using OrderServiceQuery.Core.Consumers;
using OrderServiceQuery.Core.Producer;
using OrderServiceQuery.Infrastructure.Consumer;
using OrderServiceQuery.Infrastructure.Consumers;
using OrderServiceQuery.Infrastructure.Middlewares;
using OrderServiceQuery.Infrastructure.Producers;
using OrderServiceQuery.Infrastructure.Registrations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiVersioning(options =>
        {
            // Specify how to version the API
            options.AssumeDefaultVersionWhenUnspecified = true; // Use this version when no version is specified
            options.DefaultApiVersion = new ApiVersion(1, 0); // Default version
            options.ReportApiVersions = true; // Include API version in response headers
            //options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version"); // Read version from header
            options.ApiVersionReader = new MediaTypeApiVersionReader("version");
        });

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
builder.Services.RegisterAuthentication();
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

app.UseMiddleware<UnhandledExceptionLoggingMiddleware>();

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

app.UseAuthorization();





app.Run();
