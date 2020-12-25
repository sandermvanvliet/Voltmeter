using System;
using System.Collections.Generic;
using System.Linq;
using Vibrant.InfluxDB.Client;
using Vibrant.InfluxDB.Client.Rows;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class InfluxDBEnvironmentDiscovery : IEnvironmentDiscovery
    {
        private readonly IInfluxClient _client;

        public InfluxDBEnvironmentDiscovery(IInfluxClient client)
        {
            _client = client;
        }

        public Environment[] Discover()
        {
            try
            {
                var result = _client
                    .ShowTagValuesAsync("metrics", "jedlix.environment", "health_test.self_status.count")
                    .GetAwaiter()
                    .GetResult();

                var environmentNames = result
                    .Series[0]
                    .Rows
                    .Select(tv => tv.Value)
                    .ToArray();

                var environments = new List<Environment>();

                foreach (var environment in environmentNames)
                {
                    var query =
                        $"show tag values from \"health_test.self_status.count\" with key=\"jedlix.customer\" where \"jedlix.environment\"='{environment}'";
                    
                    var values = _client
                        .ReadAsync<TagValueRow>("metrics",query)
                        .GetAwaiter()
                        .GetResult();

                    var customers = values
                        .Results[0]
                        .Series[0]
                        .Rows
                        .Select(tv => new Environment
                        {
                            Name = $"{environment}-{tv.Value.ToLower()}"
                        })
                        .ToArray();

                    environments.AddRange(customers);
                }

                return environments.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}