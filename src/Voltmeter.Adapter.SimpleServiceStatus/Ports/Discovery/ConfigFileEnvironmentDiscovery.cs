using System.Linq;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.SimpleServiceStatus.Ports.Discovery
{
    internal class ConfigFileEnvironmentDiscovery : IEnvironmentDiscovery
    {
        private readonly Settings _settings;

        public ConfigFileEnvironmentDiscovery(Settings settings)
        {
            _settings = settings;
        }

        public Environment[] Discover()
        {
            return _settings
                .Environments
                .Select(e => new Environment {Name = e})
                .ToArray();
        }
    }
}