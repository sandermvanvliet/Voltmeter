using System;
using System.Net;
using System.Net.Http;
using Codenizer.HttpClient.Testable;
using FluentAssertions;
using Voltmeter.Adapter.SimpleServiceStatus.Ports.Providers;
using Xunit;

namespace Voltmeter.Adapter.SimpleServiceStatus.Tests.Unit
{
    public class WhenGettingServiceStatus
    {
        private readonly ServiceStatusProvider _provider;
        private readonly TestableMessageHandler _handler;

        public WhenGettingServiceStatus()
        {
            _handler = new Codenizer.HttpClient.Testable.TestableMessageHandler();

            var httpClient = new HttpClient(_handler);

            _provider = new ServiceStatusProvider(httpClient);
        }

        [Fact]
        public void GivenServiceIsNull_ArgumentNullExceptionIsThrown()
        {
            Action action = () => _provider.ProvideFor(null);

            action
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenAService_ProviderCallsCanaryEndpoint()
        {
            _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            _handler
                .Requests
                .Should()
                .Contain(r => r.RequestUri.PathAndQuery == "/service/healthcheck/asg");
        }

        [Fact]
        public void GivenCanaryResponseIsOk_HealthyServiceStatusIsReturned()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck/asg")
                .With(HttpStatusCode.OK);

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Health
                .Should()
                .Be(ServiceHealth.Healthy);
        }

        [Fact]
        public void GivenCanaryResponseIsServiceUnavailable_HealthyServiceStatusIsReturned()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck/asg")
                .With(HttpStatusCode.ServiceUnavailable);

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Health
                .Should()
                .Be(ServiceHealth.Unhealthy);
        }
    }
}