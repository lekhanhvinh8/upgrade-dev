
using Microsoft.Extensions.Diagnostics.HealthChecks;


namespace MonitorService
{
    public class AlwaysHealthyHealthCheck : IHealthCheck
    {
        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(HealthCheckResult.Healthy("Always Healthy"));
        }
    }

}
