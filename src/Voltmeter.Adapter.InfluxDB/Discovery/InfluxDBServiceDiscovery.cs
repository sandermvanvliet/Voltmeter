using System.Collections.Generic;
using System.Linq;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.InfluxDB.Discovery
{
    internal class InfluxDBServiceDiscovery : IServiceDiscovery
    {
        private readonly InfluxDbCache _cache;

        public InfluxDBServiceDiscovery(InfluxDbCache cache)
        {
            _cache = cache;
        }

        public Service[] DiscoverServicesIn(Environment environment)
        {
            var current = _cache.Get<List<SelfTestMeasurement>>("self_tests");

            if (current == null)
            {
                return new Service[0];
            }

            var environmentName = environment.Name.Split('-')[0];
            var customerName = environment.Name.Split('-')[1];

            var services = current
                .Where(c => c.Environment == environmentName && c.Customer == customerName)
                .Select(c => $"{c.Domain.ToLower()}-{c.Name.ToLower()}")
                .Distinct()
                .Select(x => new Service {Environment = environment, Name = x})
                .ToArray();

            return services;
        }
    }
}