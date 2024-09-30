using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using OrderServiceCommand.Core.Configurations;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Filters;
using Serilog.Sinks.OpenTelemetry;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterLoggingExtension
    {
        public static void RegisterLogging(this IServiceCollection services, KafkaLoggingConfig kafkaLoggingConfig, string otlpEndpoint)
        {
            var logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .Filter.ByExcluding( e =>
                    Matching.FromSource("System.Net.Http.HttpClient.OtlpMetricExporter.ClientHandler")(e)
                    || Matching.FromSource("System.Net.Http.HttpClient.OtlpMetricExporter.LogicalHandler")(e)
                    || Matching.FromSource("System.Net.Http.HttpClient.OtlpTraceExporter.ClientHandler")(e)
                    || Matching.FromSource("System.Net.Http.HttpClient.OtlpTraceExporter.LogicalHandler")(e)
                )
                .Filter.ByExcluding(e => 
                    FilterSourceContext("System.Net.Http.HttpClient.OtlpMetricExporter.ClientHandler")(e)
                    || FilterSourceContext("System.Net.Http.HttpClient.OtlpMetricExporter.LogicalHandler")(e)
                    || FilterSourceContext("System.Net.Http.HttpClient.OtlpTraceExporter.ClientHandler")(e)
                    || FilterSourceContext("System.Net.Http.HttpClient.OtlpTraceExporter.LogicalHandler")(e)
                )
                .Enrich.FromLogContext()
                .WriteTo.Sink(new KafkaSink(kafkaLoggingConfig))
                .WriteTo.OpenTelemetry(opt => {
                    opt.Endpoint = otlpEndpoint + "/logs";
                    opt.Protocol = OtlpProtocol.HttpProtobuf;
                    opt.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = DiagnosticsConfig.ServiceName
                    };
                    opt.IncludedData = IncludedData.TraceIdField
                        | IncludedData.SpanIdField
                        | IncludedData.MessageTemplateTextAttribute
                        | IncludedData.MessageTemplateMD5HashAttribute;
                })
                .CreateLogger();

            Log.Logger = logger;

            services.AddLogging(loggingBuilder => {
                // Clear console and any other logging factory
                //loggingBuilder.ClearProviders();

                loggingBuilder
                    .AddSerilog(logger);
            });
        }

        private static Func<LogEvent, bool> FilterSourceContext(string sourceContext)
        {
            //return FilterFromProperty("SourceContext", sourceContext);
            return FilterFromProperty("System.SourceContext", sourceContext);
        }

        private static Func<LogEvent, bool> FilterFromProperty(string propertyName, string propValue)
        {
            return logEvent =>
            {
                try
                {
                    var getResult = logEvent.Properties.TryGetValue(propertyName, out var value);

                    if (getResult && value is ScalarValue sv)
                    {
                        var scalarValue = sv.Value;

                        if(scalarValue != null)
                        {
                            var result = (string)scalarValue == propValue;
                            return result;
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignoring the exception
                }

                return false;
            };
        }
    }

    public class KafkaSink : ILogEventSink
    {
        private readonly ProducerConfig _producerConfig;
        private readonly string _topic;
        private readonly IProducer<string, string> _producer;

        public KafkaSink(KafkaLoggingConfig config)
        {
            _producerConfig = new ProducerConfig { BootstrapServers = config.BootstrapServers };
            _topic = config.Topic;
            _producer = new ProducerBuilder<string, string>(_producerConfig).Build();
        }

        public void Emit(LogEvent logEvent)
        {
            try
            {
                var properties = new Dictionary<string, object?>(logEvent.Properties.Count);
                foreach (var property in logEvent.Properties)
                {
                    var objValue = property.Value.ToString();
                    properties[property.Key] = objValue;
                }

                var logMessage = new { 
                    Message = logEvent.RenderMessage(),
                    Properties = properties
                };

                var serializedlogMessage = JsonConvert.SerializeObject(logMessage);

                var message = new Message<string, string> { Key = Guid.NewGuid().ToString(), Value = serializedlogMessage };
                var result = this._producer.ProduceAsync(_topic, message);
            }
            catch (Exception ex)
            {
                Console.WriteLine("KafkaSinkFail: " + ex.Message);
            }
        }


        public void Flush()
        {
            this._producer.Flush();
        }

        
    }

    public class KafkaLoggingConfig
    {
        public string BootstrapServers { get; set; }
        public string Topic { get; set; }
    }
}