using System;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Voltmeter.Ports.Providers;

namespace Voltmeter.Adapter.SimpleServiceStatus.Ports.Providers
{
    internal class ServiceDependenciesProvider : IServiceDependenciesProvider
    {
        private readonly HttpClient _httpClient;
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() },
            NullValueHandling = NullValueHandling.Include
        };

        public ServiceDependenciesProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public DependencyStatus[] ProvideFor(Service service)
        {
            var canaryEndpoint = new Uri(service.Location, "/service/healthcheck");

            var response = _httpClient
                .GetAsync(canaryEndpoint)
                .GetAwaiter()
                .GetResult();

            if (!response.IsSuccessStatusCode)
            {
                return new DependencyStatus[0];
            }

            var serialized = response
                .Content
                .ReadAsStringAsync()
                .GetAwaiter()
                .GetResult();

            if (string.IsNullOrWhiteSpace(serialized))
            {
                return new DependencyStatus[0];
            }

            var healthTests = JsonConvert.DeserializeObject<HealthCheckResponse>(serialized, SerializerSettings);

            return healthTests
                .Tests
                .Select(ht =>
                    new DependencyStatus
                    {
                        Name = GetDependencyName(ht),
                        IsHealthy = ht.TestResult == "passed"
                    })
                .ToArray();
        }

        private static string GetDependencyName(HealthTest ht)
        {
            if (ht.TestName == "http" && !string.IsNullOrWhiteSpace(ht.TestIdentifier))
            {
                return ht.TestIdentifier;
            }

            if (!string.IsNullOrWhiteSpace(ht.TestIdentifier))
            {
                return $"{ht.TestName}-{ht.TestIdentifier}";
            }

            return ht.TestName;
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        // Because it's only used for deserialization
        private class HealthCheckResponse
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public HealthTest[] Tests { get; set; }
        }

        // ReSharper disable once ClassNeverInstantiated.Local
        // Because it's only used for deserialization
        private class HealthTest
        {
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string TestResult { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string TestName { get; set; }
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string TestIdentifier { get; set; }
        }
    }
}