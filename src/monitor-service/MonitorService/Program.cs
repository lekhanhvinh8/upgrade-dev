using MonitorService;
using MonitorService.Configurations;
using HealthChecks.UI.Client;
using HealthChecks.UI.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSecretVault();

builder.Services.AddControllers();
builder.Services.AddControllersWithViews(); 

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAppAuthentication();

builder.Services.AddSingleton<IAuthorizationHandler, AppConfigAuthorizationHandler>();
builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .AddRequirements(new AppConfigAuthorizationRequirement())
        .Build();
});


builder.Services.AddHealthChecks()
        .AddCheck<AlwaysHealthyHealthCheck>("AlwaysHealthyHealthCheck");

builder.Services.AddTransient<IHealthCheckCollectorInterceptor, ControlledHealthCheckCollectorInterceptor>();

var healthCheckUIConfig = AddSecretVaultExtension.VaultConfiguration!.GetSection("HealthChecksUI").Get<HealthChecksUI>();

builder.Services.AddHealthChecksUI(setup =>
{
    setup.UseApiEndpointHttpMessageHandler(sp =>
    {
        return new HttpClientHandler
        {
            ClientCertificateOptions = ClientCertificateOption.Manual,
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => { return true; }
        };
    });

    setup.AddHealthCheckEndpoint("self-healthcheck", "/hc");

    foreach (var healthCheck in healthCheckUIConfig.HealthChecks!)
    {
        setup.AddHealthCheckEndpoint(healthCheck.Name!, healthCheck.Uri!);
    }

    setup.SetEvaluationTimeInSeconds((int)healthCheckUIConfig.EvaluationTimeInSeconds!);
    setup.SetMinimumSecondsBetweenFailureNotifications((int)healthCheckUIConfig.MinimumSecondsBetweenFailureNotifications!);
    setup.MaximumHistoryEntriesPerEndpoint((int)healthCheckUIConfig.MaximumHistoryEntriesPerEndpoint!);

    foreach (var webhook in healthCheckUIConfig.Webhooks!)
    {
        setup.AddWebhookNotification(webhook.Name!, webhook.Uri!, webhook.Payload!, webhook.RestoredPayload!);
    }

})
.AddInMemoryStorage();

var app = builder.Build();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapHealthChecks("/hc", new HealthCheckOptions()
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

app.MapHealthChecksUI(config =>
{
    config.UIPath = "/"; // Path to HealthCheck UI
});

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
