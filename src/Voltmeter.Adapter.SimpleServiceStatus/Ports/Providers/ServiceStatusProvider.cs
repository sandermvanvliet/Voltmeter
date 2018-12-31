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
            var canaryEndpoint = new Uri(service.Location, "/service/healthcheck/asg");

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
    }
}