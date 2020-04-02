using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voltmeter.Adapter.SimpleServiceStatus.Ports.Discovery;
using Voltmeter.Adapter.SimpleServiceStatus.Ports.Providers;
using Voltmeter.Adapter.SimpleServiceStatus.Ports.Storage;
using Voltmeter.Ports.Discovery;
using Voltmeter.Ports.Providers;
using Voltmeter.Ports.Storage;

namespace Voltmeter.Adapter.SimpleServiceStatus
{
    public static class AdapterExtensions
    {
        public static IServiceCollection AddSimpleServiceStatus(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(_ =>
                {
                    var client = new HttpClient();

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Client-Name", "Voltmeter");
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("voltmeter", "1.0"));
                    client.Timeout = TimeSpan.FromSeconds(1);

                    return client;
                });

            serviceCollection
                .AddSingleton(_ =>
                {
                    var settings = new Settings();

                    var configuration = _.GetService<IConfiguration>();
                    var section = configuration.GetSection("Voltmeter");

                    section.Bind(settings);

                    return settings;
                });

            serviceCollection
                .AddTransient<IEnvironmentDiscovery, ConfigFileEnvironmentDiscovery>()
                .AddTransient<IServiceDiscovery, ConfigFIleServiceDiscovery>()
                .AddTransient<IServiceStatusProvider, ServiceStatusProvider>()
                .AddTransient<IServiceDependenciesProvider, ServiceDependenciesProvider>();

            serviceCollection
                .AddSingleton<IEnvironmentStatusStore, EnvironmentStatusStore>();

            return serviceCollection;
        }
    }
}