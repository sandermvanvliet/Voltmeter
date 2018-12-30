using System.Linq;
using Voltmeter.Ports.Discovery;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class EnvironmentStatusProvider: IEnvironmentStatusProvider
    {
        private readonly IServiceDiscovery _serviceDiscovery;

        public EnvironmentStatusProvider(IServiceDiscovery serviceDiscovery)
        {
            _serviceDiscovery = serviceDiscovery;
        }

        public ApplicationStatus[] ProvideFor(string environmentName)
        {
            var services = _serviceDiscovery.DiscoverServicesIn(environmentName);

            return services
                .Select(s => new ApplicationStatus
                {
                    IsHealthy = true,
                    Name = s.Name,
                    Location = s.Location
                })
                .ToArray();
        }
    }
}