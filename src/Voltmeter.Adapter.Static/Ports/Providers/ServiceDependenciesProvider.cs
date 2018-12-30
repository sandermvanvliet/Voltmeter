using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.Static.Ports.Providers
{
    internal class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        public DependencyStatus[] ProvideFor(Service service)
        {
            return new DependencyStatus[0];
        }
    }
}