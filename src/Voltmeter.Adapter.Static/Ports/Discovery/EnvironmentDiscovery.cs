using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.Static.Ports.Discovery
{
    internal class EnvironmentDiscovery : IEnvironmentDiscovery
    {
        public string[] Discover()
        {
            return new[]
            {
                "production",
                "staging",
                "acceptance",
                "test"
            };
        }
    }
}