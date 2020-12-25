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
                    .Select(tv => new DependencyStatus {Name = tv.Value, IsHealthy = true})
                    .ToArray();

                return dependencies;
            }

            return new DependencyStatus[0];
        }
    }
}