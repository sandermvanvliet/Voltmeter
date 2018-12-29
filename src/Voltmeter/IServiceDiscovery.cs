using System;

namespace Voltmeter
{
    public interface IServiceDiscovery
    {
        Uri[] DiscoverServicesIn(string environment);
    }
}