using System;
using System.Linq;

namespace Voltmeter
{
    public class ServiceDiscovery : IServiceDiscovery
    {
        private static readonly string[] Services = {
            "user"
        };

        public Uri[] DiscoverServicesIn(string environment)
        {
            return Services
                .Select(service => new Uri($"https://{environment}-{service}-api.azurewebsites.net"))
                .ToArray();
        }
    }
}