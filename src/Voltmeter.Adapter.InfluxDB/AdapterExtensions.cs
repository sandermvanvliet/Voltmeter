using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vibrant.InfluxDB.Client;
using Voltmeter.Adapter.InfluxDB.Discovery;
using Voltmeter.Adapter.InfluxDB.Providers;
using Voltmeter.Adapter.InfluxDB.Storage;
using Voltmeter.Ports.Discovery;
using Voltmeter.Ports.Providers;
using Voltmeter.Ports.Storage;

namespace Voltmeter.Adapter.InfluxDB
{
    public static class AdapterExtensions
    {
        public static IServiceCollection AddInfluxServiceStatus(this IServiceCollection serviceCollection)
        {
            serviceCollection
                .AddSingleton(_ =>
                {
                    var settings = new InfluxDbSettings();

                    var configuration = _.GetService<IConfiguration>();
                    var section = configuration.GetSection("Voltmeter").GetSection("InfluxDb");

                    section.Bind(settings);

                    return settings;
                });

            serviceCollection
                .AddSingleton<IInfluxClient>(serviceProvider =>
                {
                    var settings = serviceProvider.GetRequiredService<InfluxDbSettings>();

                    return new InfluxClient(
                        new Uri(settings.Url),
                        settings.Username,
                        settings.Password);
                });

            serviceCollection
                .AddTransient<IEnvironmentDiscovery, InfluxDBEnvironmentDiscovery>()
                .AddTransient<IServiceDiscovery, InfluxDBServiceDiscovery>()
                .AddTransient<IServiceStatusProvider, ServiceStatusProvider>()
                .AddTransient<IServiceDependenciesProvider, ServiceDependenciesProvider>();

            serviceCollection
                .AddSingleton<IEnvironmentStatusStore, EnvironmentStatusStore>();

            return serviceCollection;
        }
    }
}
