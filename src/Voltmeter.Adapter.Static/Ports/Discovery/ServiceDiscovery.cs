using System;
using System.Linq;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.Static.Ports.Discovery
{
    internal class ServiceDiscovery : IServiceDiscovery
    {
        private static readonly string[] Services =
        {
            "user"
        };

        public Service[] DiscoverServicesIn(Environment environment)
        {
            return Services
                .Select(service =>
                    new Service
                    {
                        Location = new Uri($"https://{environment}-{service}-api.azurewebsites.net"),
                        Name = service,
                        Environment = environment
                    })
                .ToArray();
        }
    }
}