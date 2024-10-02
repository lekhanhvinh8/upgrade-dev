using System.Threading.RateLimiting;
using FluentValidation;
using Polly;
using Polly.Extensions.Http;
using ServiceInfoService.Sevices.Http;
using ServiceInfoService.Sevices.IAM;
using static OrderServiceQuery.API.Controllers.OrderController.ServiceInfoController;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddControllers();

builder.Services.AddScoped<IValidator<Input>, UserValidator>();

// Define the circuit breaker policy
var circuitBreakerPolicy = 


builder.Services.AddHttpClient<IHttpService, HttpService>()
    .ConfigureHttpClient(client =>
    {
        client.Timeout = TimeSpan.FromSeconds(30); // Set a timeout
    })
    .AddPolicyHandler(
        HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.BadGateway)
        .WaitAndRetryAsync(5, retryAttempt => TimeSpan.FromSeconds(0.5)) 
    )
    .AddPolicyHandler(
        HttpPolicyExtensions
        .HandleTransientHttpError() // Handles transient errors
        .CircuitBreakerAsync(
            handledEventsAllowedBeforeBreaking: 3, // Break after 2 consecutive errors
            durationOfBreak: TimeSpan.FromSeconds(20))
    ); // Add circuit breaker policy;

builder.Services.AddScoped<IIAMService, IAMService>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: httpContext.User.Identity?.Name ?? httpContext.Request.Headers.Host.ToString(),
            factory: partition => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 60, // Maximum number of requests allowed
                Window = TimeSpan.FromMinutes(1), // Time window (e.g., 1 minute)
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0 // Number of requests allowed to be queued
            }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

app.UseRateLimiter();

app.MapControllers();

app.Run();



