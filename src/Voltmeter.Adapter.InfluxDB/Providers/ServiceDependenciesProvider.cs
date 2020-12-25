using System;
using System.Linq;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB.Client.Rows;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.InfluxDB.Providers
{
    public class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        private readonly IInfluxClient _client;

        public ServiceDependenciesProvider(IInfluxClient client)
        {
            _client = client;
        }

        public DependencyStatus[] ProvideFor(Service service)
        {
            var parts = service.Name.Split('-');

            if (parts.Length != 2)
            {
                return new DependencyStatus[0];
            }

            var domain = parts[0];
            var name = parts[1];

            var environmentName = service.Environment.Name.Split('-')[0];
            var customerName = service.Environment.Name.Split('-')[1];

            var query = $"show tag values from \"health_test.test.count\" with key=\"test\" where \"jedlix.domain\"='{domain}' AND \"jedlix.name\"='{name}' AND \"jedlix.environment\"='{environmentName}' AND \"jedlix.customer\"='{customerName}'";
            var values = _client
                .ReadAsync<TagValueRow>("metrics", query)
                .GetAwaiter()
                .GetResult();

            if (values.Results.Count > 0 && values.Results[0].Series.Count > 0)
            {
                var dependencies = values
                    .Results[0]
                    .Series[0]
                    .Rows
                    .Select(tv => new DependencyStatus {Name = tv.Value })
                    .ToArray();

                foreach (var dep in dependencies)
                {
                    var healthStatus = GetDependencyHealth(dep, customerName, environmentName, domain, name);
                    dep.Health = healthStatus;
                }

                return dependencies;
            }

            return new DependencyStatus[0];
        }

        private ServiceHealth GetDependencyHealth(DependencyStatus dep, string customerName, string environmentName, string domain, string serviceName)
        {
            var time = DateTime.UtcNow.AddMinutes(-4);

            var query =
                $"select value from \"health_test.test.count\" where \"jedlix.environment\"='{environmentName}' AND \"jedlix.customer\"='{customerName}' AND time>'{time:yyyy-MM-ddTHH:mm:ssZ}' AND \"jedlix.name\"='{serviceName}' AND \"jedlix.domain\"='{domain}' AND \"test\"='{dep.Name}' ORDER BY time DESC LIMIT 5";
                
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
                    return ServiceHealth.Healthy;
                }

                if (averageHealth >= 0.75)
                {
                    return ServiceHealth.Degraded;
                }
            }

            return ServiceHealth.Unhealthy;
        }
    }
}