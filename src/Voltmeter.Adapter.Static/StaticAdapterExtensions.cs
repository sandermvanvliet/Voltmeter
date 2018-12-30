using Microsoft.Extensions.DependencyInjection;
using Voltmeter.Adapter.Static.Ports.Discovery;
using Voltmeter.Adapter.Static.Ports.Providers;
using Voltmeter.Adapter.Static.Ports.Storage;
using Voltmeter.Ports.Discovery;
using Voltmeter.Ports.Providers;
using Voltmeter.Ports.Storage;

namespace Voltmeter.Adapter.Static
{
    public static class StaticAdapterExtensions
    {
        public static IServiceCollection AddStaticAdapter(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddTransient<IEnvironmentDiscovery, EnvironmentDiscovery>()
                .AddTransient<IServiceDiscovery, ServiceDiscovery>()
                .AddTransient<IEnvironmentStatusProvider, EnvironmentStatusProvider>(); ;

            serviceCollection
                .AddSingleton<IEnvironmentStatusStore, EnvironmentStatusStore>();

            return serviceCollection;
        }
    }
}