using System;
using System.Linq;
using Vibrant.InfluxDB.Client;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.InfluxDB.Providers
{
    public class ServiceStatusProvider : IServiceStatusProvider
    {
        private readonly IInfluxClient _client;

        public ServiceStatusProvider(IInfluxClient client)
        {
            _client = client;
        }

        public ServiceStatus ProvideFor(Service service)
        {
            var customer = service.Environment.Name.Split('-')[0];
            var environment = service.Environment.Name.Split('-')[1];
            var time = DateTime.UtcNow.AddMinutes(-4);
            var domain = service.Name.Split('-')[0];
            var serviceName = service.Name.Split('-')[1];

            var query =
                $"select value from \"health_test.self_status.count\" where \"jedlix.environment\"='{environment}' AND \"jedlix.customer\"='{customer}' AND time>'{time:yyyy-MM-ddTHH:mm:ssZ}' AND \"jedlix.name\"='{serviceName}' AND \"jedlix.domain\"='{domain}' ORDER BY time DESC LIMIT 5";
                
            var result = _client
                .ReadAsync<HealthTestMeasurement>("metrics", query)
                .GetAwaiter()
                .GetResult();

            if (result.Results.Count > 0 && result.Results[0].Succeeded)
            {
                var averageHealth = result
                    .Results[0]
                    .Series[0]
                    .Rows
                    .Select(r => r.Value)
                    .Average();

                if (Math.Abs(averageHealth - 1) < 0.01)
                {
                    return ServiceStatus.HealthyFrom(service);
                }

                if (averageHealth >= 0.75)
                {
                    return ServiceStatus.DegradedFrom(service);
                }
            }

            return ServiceStatus.UnhealthyFrom(service);
        }
    }
}