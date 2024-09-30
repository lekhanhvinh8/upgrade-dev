using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OrderServiceQuery.Core.Configurations
{
    public class DiagnosticsConfig {
        public static string ServiceName = "order-service-query";
        public static Meter Meter = new Meter(ServiceName);
        public ActivitySource ActivitySource  = new ActivitySource(ServiceName);
    }
}

