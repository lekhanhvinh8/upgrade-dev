namespace MonitorService.Configurations
{
    public static class AddSecretVaultExtension
    {
        public static IConfigurationRoot? VaultConfiguration;

        public static IServiceCollection AddSecretVault(this IServiceCollection services)
        {
            try
            {
                var vaultConfiguration = GetVaultConfiguration();
                Console.WriteLine("vaultConfiguration: " + vaultConfiguration.GetDebugView());

                services.Configure<HealthChecksUI>(vaultConfiguration.GetSection("HealthChecksUI"));
                services.Configure<HealthCheckController>(vaultConfiguration.GetSection("HealthCheckController"));


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

            VaultConfiguration = vaultConfiguration;

            return vaultConfiguration;
        }
    }
}