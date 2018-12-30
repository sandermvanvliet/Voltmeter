using System;

namespace Voltmeter.Ports.Discovery
{
    public interface IServiceDiscovery
    {
        Uri[] DiscoverServicesIn(string environment);
    }
}