using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.InfluxDB.Providers
{
    public class ServiceStatusProvider : IServiceStatusProvider
    {
        public ServiceStatus ProvideFor(Service service)
        {
            return ServiceStatus.HealthyFrom(service);
        }
    }
}