
using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OrderServiceCommand.Infrastructure.ConfigurationValues;

namespace OrderServiceCommand.Infrastructure.Registrations
{
    public static class RegisterConfigurationValueExtension
    {
        public static ConnectionStrings? ConnectionStrings;

        public static IServiceCollection RegisterConfigurationValue(this IServiceCollection services)
        {
            
            try
            {
                var vaultConfiguration = GetVaultConfiguration();
                Console.WriteLine("vaultConfiguration: " + vaultConfiguration.GetDebugView());

                services.Configure<MongoDbConfig>(vaultConfiguration.GetSection("orderServiceCommand:mongoDbConfig"));

                var connectionStringSection = vaultConfiguration.GetSection("orderServiceCommand:ConnectionStrings");
                ConnectionStrings = connectionStringSection.Get<ConnectionStrings>();
                services.Configure<ConnectionStrings>(connectionStringSection);

                var producerConfigSection = vaultConfiguration.GetSection("orderServiceCommand:ProducerConfig");
                services.Configure<ProducerConfig>(producerConfigSection);
            }
            catch (Exception ex)
            {
                Console.WriteLine("AddSecretVault fail: " + ex.Message + ", Stack Trace: " + ex.StackTrace);
            }

            return services;
        }


        public static string GetVaultConfigPathFile()
        {
            // This path is config by system team. The actually file is from https://isc-secret.fpt.net. System config the file to appear in this path
            
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower();

            if(string.IsNullOrEmpty(environment))
            {
                Console.WriteLine("environment variable not defined: ASPNETCORE_ENVIRONMENT Is null or empty");
            }

            var pathFile = "";

            if(environment == "local")
            {
                pathFile = "C:/projects/upgrade-dev-ftel-project/infras/vault/output/secret.json";
            }
            else
            {
                pathFile = String.Format("/vault/secrets/configuration.{0}.json", environment);
            }

            return pathFile;
        }

        public static IConfigurationRoot GetVaultConfiguration()
        {
            var pathFile = GetVaultConfigPathFile();
            
            IConfigurationRoot vaultConfiguration = new ConfigurationBuilder()
                .AddJsonFile(pathFile, 
                    optional: true, 
                    reloadOnChange: true)
                .Build();

            return vaultConfiguration;
        }
    }
}