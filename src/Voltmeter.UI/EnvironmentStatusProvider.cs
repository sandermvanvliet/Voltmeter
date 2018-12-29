using System.Linq;

namespace Voltmeter.UI
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
                    Name = s.ToString(),
                    Location = s
                })
                .ToArray();
        }
    }
}