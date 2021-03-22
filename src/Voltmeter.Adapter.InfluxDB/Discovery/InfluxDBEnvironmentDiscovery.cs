using System;
using System.Linq;
using Serilog;
using Vibrant.InfluxDB.Client;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class InfluxDBEnvironmentDiscovery : IEnvironmentDiscovery
    {
        private readonly IInfluxClient _client;
        private readonly ILogger _logger;
        private readonly InfluxDbCache _cache;

        public InfluxDBEnvironmentDiscovery(IInfluxClient client, ILogger logger, InfluxDbCache cache)
        {
            _client = client;
            _logger = logger;
            _cache = cache;
        }

        public Environment[] Discover()
        {
            try
            {
                var timeUpper = DateTimeProvider.UtcNow();
                var timeLower = timeUpper.AddMinutes(-10);

                var query =
                    $"select value, \"jedlix.environment\", \"jedlix.customer\", \"jedlix.name\", \"jedlix.domain\" from \"health_test.self_status.count\" where time > '{timeLower:yyyy-MM-ddTHH:mm:ssZ}' AND time < '{timeUpper:yyyy-MM-ddTHH:mm:ssZ}'";

                var result = _client
                    .ReadAsync<SelfTestMeasurement>("metrics", query)
                    .GetAwaiter()
                    .GetResult();

                if (!result.Results.Any() || !result.Results[0].Succeeded)
                {
                    if (result.Results.Any())
                    {
                        _logger.Warning("InfluxDB query failed: {Reason}", result.Results[0].ErrorMessage);
                    }
                    else
                    {
                        _logger.Warning("InfluxDB query failed");
                    }

                    return new Environment[0];
                }

                var current = result
                    .Results[0]
                    .Series[0]
                    .Rows
                    .ToList();

                _cache.Store("self_tests", current);

                return current
                    .Select(c => $"{c.Environment.ToLower()}-{c.Customer.ToLower()}")
                    .Distinct()
                    .Select(e => new Environment {Name = e})
                    .ToArray();
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "InfluxDB query failed");

                return new Environment[0];
            }
        }
    }
}