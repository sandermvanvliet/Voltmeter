using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        public Dependency[] ProvideFor(Service service)
        {
            return new Dependency[0];
        }
    }
}