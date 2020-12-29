using System;
using System.Collections.Generic;
using System.Linq;
using Vibrant.InfluxDB.Client;
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

            var time = DateTime.UtcNow.AddMinutes(-10);

            var domain = parts[0];
            var name = parts[1];
            var environmentName = service.Environment.Name.Split('-')[0];
            var customerName = service.Environment.Name.Split('-')[1];

            var query = $"select value, test from \"health_test.test.count\" where \"jedlix.domain\"='{domain}' AND \"jedlix.name\"='{name}' AND \"jedlix.environment\"='{environmentName}' AND \"jedlix.customer\"='{customerName}' AND time > '{time:yyyy-MM-ddTHH:mm:ssZ}'";
            var values = _client
                .ReadAsync<HealthTestMeasurement>("metrics", query)
                .GetAwaiter()
                .GetResult();

            if (values.Results.Any() && values.Results[0].Succeeded)
            {
                return values
                    .Results[0]
                    .Series[0]
                    .Rows
                    .GroupBy(
                        m => m.Test,
                        m => m.Value,
                        (key, healthValues) => new DependencyStatus
                        {
                            Name = key,
                            Health = GetHealthOf(healthValues)
                        })
                    .ToArray();
            }

            return new DependencyStatus[0];
        }

        private ServiceHealth GetHealthOf(IEnumerable<int> values)
        {
            if (values.Any())
            {
                var averageHealth = values.Average();

                if (Math.Abs(averageHealth - 1) < 0.01)
                {
                    return ServiceHealth.Healthy;
                }

                if (averageHealth >= 0.75)
                {
                    return ServiceHealth.Degraded;
                }

                return ServiceHealth.Unhealthy;
            }

            return ServiceHealth.Unknown;
        }
    }
}