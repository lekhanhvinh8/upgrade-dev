namespace MonitorService.Configurations
{
    public class Monitoring
    {
        public HealthChecksUI? HealthChecksUI { get; set; }
        public HealthCheckController? HealthCheckController { get; set; }

    }

    public class HealthChecksUI
    {
        public List<HealthCheck>? HealthChecks { get; set; }
        public List<Webhook>? Webhooks { get; set; }
        public int? EvaluationTimeInSeconds { get; set; }
        public int? MinimumSecondsBetweenFailureNotifications { get; set; }
        public int? MaximumHistoryEntriesPerEndpoint { get; set; }
        
        public HealthChecksUI()
        {
            Webhooks = new List<Webhook>();
        }
    }

    public class HealthCheckController
    {
        public bool? Start { get; set; }
        public int? IntervalBetweenEndpointCalls { get; set; }
        public int? IntervalBetweenBatches { get; set; }
    }


    public class HealthCheck
    {
        public string? Name { get; set; }
        public string? Uri { get; set; }
    }

    public class Webhook
    {
        public string? Name { get; set; }
        public string? Uri { get; set; }
        public string? Payload { get; set; }
        public string? RestoredPayload { get; set; }
    }
}