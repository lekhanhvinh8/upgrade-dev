namespace OrderServiceQuery.Infrastructure.ConfigurationValues
{
    public class ConnectionStrings
    {
        public string? Otel { get; set; }
        public string? SqlServer { get; set; }
        public string? RedisCache { get; set; }
        public string? BootstrapServers { get; set; }
    }
}