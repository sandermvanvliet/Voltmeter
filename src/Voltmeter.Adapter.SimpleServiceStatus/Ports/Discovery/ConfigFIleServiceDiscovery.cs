using System;
using System.Linq;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.SimpleServiceStatus.Ports.Discovery
{
    internal class ConfigFIleServiceDiscovery : IServiceDiscovery
    {
        private readonly Settings _settings;

        public ConfigFIleServiceDiscovery(Settings settings)
        {
            _settings = settings;
        }

        public Service[] DiscoverServicesIn(Environment environment)
        {
            return _settings
                .Services
                .Select(service => new Service
                {
                    Environment = environment,
                    Location = new Uri(ApplyPattern(service.LocationPattern, environment, service)),
                    Name = service.Name
                })
                .ToArray();
        }

        private string ApplyPattern(string pattern, Environment environment, ServiceSetting service)
        {
            return pattern
                .Replace("{environment}", environment.Name)
                .Replace("{service}", service.Name);
        }
    }
}