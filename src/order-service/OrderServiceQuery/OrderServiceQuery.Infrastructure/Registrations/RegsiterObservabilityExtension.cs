
using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OrderServiceQuery.Core.Configurations;

namespace OrderServiceCommand.Infrastructure.Registrations
{

    public static class RegsiterObservabilityExtension
    {
        public static void RegsiterObservability(this IServiceCollection services, string otlpEndpoint)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();
            if(environment == null)
            {
                environment = "undefined";
            }

            services.AddOpenTelemetry()
                    .ConfigureResource(resourceBuilder => 
                        resourceBuilder
                            .AddService(DiagnosticsConfig.ServiceName)
                            .AddAttributes(new List<KeyValuePair<string, object>> { 
                                new("deployment.environment", environment),
                            }))
                    .WithTracing(tracerProviderBuilder => tracerProviderBuilder
                        .AddSource("ConsumeProcess")
                        .ConfigureResource(resource => resource
                        .AddService(DiagnosticsConfig.ServiceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddEntityFrameworkCoreInstrumentation()
                        //.AddConfluentKafkaInstrumentation()
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new UriBuilder( otlpEndpoint + "/traces").Uri;
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        })
                    )
                    .WithMetrics(meterProviderBuilder => meterProviderBuilder
                        .AddMeter(DiagnosticsConfig.Meter.Name)
                        .ConfigureResource(resource => resource
                            .AddService(DiagnosticsConfig.ServiceName))
                        .AddAspNetCoreInstrumentation()
                        .AddHttpClientInstrumentation()
                        .AddRuntimeInstrumentation()
                        .AddOtlpExporter(otlpOptions =>
                        {
                            otlpOptions.Endpoint = new UriBuilder(otlpEndpoint + "/metrics").Uri;
                            otlpOptions.Protocol = OpenTelemetry.Exporter.OtlpExportProtocol.HttpProtobuf;
                        })
                    );

            // Define Gauges for CPU Utilization and Memory Usage
            DiagnosticsConfig.Meter.CreateObservableGauge<double>("system.cpu.utilization", () => {
                var cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total", true);
                var value = cpuCounter.NextValue();

                //Note: In most cases you need to call .NextValue() twice to be able to get the real value
                if (Math.Abs(value) <= 0.00)
                    value = cpuCounter.NextValue();
                return value;
            }, "1", "no description");
            
            DiagnosticsConfig.Meter.CreateObservableGauge<double>("system.memory.utilization", () => {
                var theMemCounter = new PerformanceCounter("Memory", "Available MBytes");
                var value = theMemCounter.NextValue();

                if (Math.Abs(value) <= 0.00)
                    value = theMemCounter.NextValue();

                return value;
            }, "1");
        }
    }
}
