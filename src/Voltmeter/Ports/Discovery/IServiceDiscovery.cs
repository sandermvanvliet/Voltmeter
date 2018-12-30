namespace Voltmeter.Ports.Discovery
{
    public interface IServiceDiscovery
    {
        Service[] DiscoverServicesIn(Environment environment);
    }
}