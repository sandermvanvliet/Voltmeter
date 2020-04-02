using System;
using System.Net.Http;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.SimpleServiceStatus.Ports.Providers
{
    internal class ServiceStatusProvider : IServiceStatusProvider
    {
        private readonly HttpClient _httpClient;

        public ServiceStatusProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public ServiceStatus ProvideFor(Service service)
        {
            if (service == null)
            {
                throw new ArgumentNullException();
            }

            var canaryEndpoint = new Uri(service.Location, "/service/healthcheck/asg");

            try
            {
                var response = _httpClient
                    .GetAsync(canaryEndpoint)
                    .GetAwaiter()
                    .GetResult();

                if (response.IsSuccessStatusCode)
                {
                    return ServiceStatus.HealthyFrom(service);
                }

                return ServiceStatus.UnhealthyFrom(service);
            }
            catch (Exception)
            {
                return ServiceStatus.UnhealthyFrom(service);
            }
        }
    }
}