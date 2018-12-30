namespace Voltmeter.Ports.Discovery
{
    public interface IServiceDiscovery
    {
        Service[] DiscoverServicesIn(string environment);
    }
}