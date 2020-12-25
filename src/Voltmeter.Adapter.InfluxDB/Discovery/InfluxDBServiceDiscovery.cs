using System;
using System.Collections.Generic;
using System.Linq;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB.Client.Rows;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class InfluxDBServiceDiscovery : IServiceDiscovery
    {
        private readonly IInfluxClient _client;

        public InfluxDBServiceDiscovery(IInfluxClient client)
        {
            _client = client;
        }

        public Service[] DiscoverServicesIn(Environment environment)
        {
            var result = _client
                .ShowTagValuesAsync("metrics", "jedlix.domain", "health_test.self_status.count")
                .GetAwaiter()
                .GetResult();

            var domains = result
                .Series[0]
                .Rows
                .Select(tv => tv.Value)
                .ToArray();

            var services = new List<Service>();

            foreach (var domain in domains)
            {
                var values = _client
                    .ReadAsync<TagValueRow>("metrics", $"show tag values from \"health_test.self_status.count\" with key=\"jedlix.name\" where \"jedlix.domain\"='{domain}'")
                    .GetAwaiter()
                    .GetResult();

                services
                    .AddRange(
                        values
                            .Results[0]
                            .Series[0]
                            .Rows
                            .Select(r => new Service
                            {
                                Environment = environment, 
                                Name = $"{domain}-{r.Value.ToLower()}"
                            })
                            .ToList());
            }

            return services.ToArray();
        }
    }
}