using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace OrderServiceCommand.Core.Configurations
{
    public class DiagnosticsConfig {
        public static string ServiceName = "order-service-command";
        public static Meter Meter = new Meter(ServiceName);
        public ActivitySource ActivitySource  = new ActivitySource(ServiceName);
    }
}

