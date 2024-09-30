
using MonitorService.Configurations;
using HealthChecks.UI.Core;
using HealthChecks.UI.Core.Data;
using Microsoft.Extensions.Options;

namespace MonitorService
{
    public class ControlledHealthCheckCollectorInterceptor : IHealthCheckCollectorInterceptor
    {
        private readonly IOptionsMonitor<HealthCheckController> _healthChecksConfig;

        public ControlledHealthCheckCollectorInterceptor(IOptionsMonitor<HealthCheckController> healthChecksConfig)
        {
            this._healthChecksConfig = healthChecksConfig;
        }

        public async ValueTask OnCollectExecuting(HealthCheckConfiguration healthCheck)
        {
            while(true)
            {
                var currentLoopValue = _healthChecksConfig.CurrentValue;
                if(currentLoopValue.Start == true)
                {
                    break;
                }
                else
                {
                    await Task.Delay(1000);
                }
            }

            var currentValue = _healthChecksConfig.CurrentValue;
            if(currentValue.IntervalBetweenEndpointCalls != null)
            {
                await Task.Delay((int)currentValue.IntervalBetweenEndpointCalls);
            }

            if(healthCheck.Name == "self-healthcheck")
            {
                if(currentValue.IntervalBetweenBatches != null)
                {
                    await Task.Delay((int)currentValue.IntervalBetweenBatches);
                }
            }

            return;
        }

        public async ValueTask OnCollectExecuted(UIHealthReport report)
        {
            return;
        }

        
    }
}