using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        public ServiceDependency[] ProvideFor(Service service)
        {
            return new ServiceDependency[0];
        }
    }
}