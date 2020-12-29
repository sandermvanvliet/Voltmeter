using System;
using System.Collections.Generic;
using System.Linq;
using Voltmeter.Adapter.InfluxDB.Discovery;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.InfluxDB.Providers
{
    internal class ServiceStatusProvider : IServiceStatusProvider
    {
        private readonly InfluxDbCache _cache;

        public ServiceStatusProvider(InfluxDbCache cache)
        {
            _cache = cache;
        }

        public ServiceStatus ProvideFor(Service service)
        {
            var environment = service.Environment.Name.Split('-')[0];
            var customer = service.Environment.Name.Split('-')[1];
            var domain = service.Name.Split('-')[0];
            var serviceName = service.Name.Split('-')[1];

            var current = _cache.Get<List<SelfTestMeasurement>>("self_tests");

            if (current == null)
            {
                return ServiceStatus.UnknownFrom(service);
            }

            var matches = current
                .Where(c => c.Customer == customer &&
                            c.Environment == environment &&
                            c.Domain == domain &&
                            c.Name == serviceName)
                .ToList();

            double averageHealth = 0;

            if (matches.Any())
            {
                averageHealth = matches.Average(c => c.Value);
            }


            if (Math.Abs(averageHealth - 1) < 0.01)
            {
                return ServiceStatus.HealthyFrom(service);
            }

            if (averageHealth >= 0.75)
            {
                return ServiceStatus.DegradedFrom(service);
            }

            return ServiceStatus.UnhealthyFrom(service);
        }
    }
}