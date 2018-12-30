using System.Linq;
using Voltmeter.Ports.Discovery;

namespace Voltmeter.Adapter.Static.Ports.Discovery
{
    internal class EnvironmentDiscovery : IEnvironmentDiscovery
    {
        public Environment[] Discover()
        {
            return new[]
                {
                    "production",
                    "staging",
                    "acceptance",
                    "test"
                }
                .Select(e => new Environment {Name = e})
                .ToArray();
        }
    }
}