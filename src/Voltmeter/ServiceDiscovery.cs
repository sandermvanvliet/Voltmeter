using System;
using System.Linq;

namespace Voltmeter
{
    public class ServiceDiscovery : IServiceDiscovery
    {
        private static readonly string[] Services = {
            "assets",
            "catalog",
            "energysuppliers",
            "user",
            "sessions",
            "savings",
            "settlement"
        };

        public Uri[] DiscoverServicesIn(string environment)
        {
            return Services
                .Select(service => new Uri($"https://{environment}-{service}-api.azurewebsites.net"))
                .ToArray();
        }
    }
}