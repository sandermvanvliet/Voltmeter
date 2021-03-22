using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using Codenizer.HttpClient.Testable;
using FluentAssertions;
using Voltmeter.Adapter.SimpleServiceStatus.Ports.Providers;
using Xunit;

namespace Voltmeter.Adapter.SimpleServiceStatus.Tests.Unit
{
    public class WhenGettingServiceDependencies
    {
        private readonly ServiceDependenciesProvider _provider;
        private readonly TestableMessageHandler _handler;

        public WhenGettingServiceDependencies()
        {
            _handler = new Codenizer.HttpClient.Testable.TestableMessageHandler();

            var httpClient = new HttpClient(_handler);

            _provider = new ServiceDependenciesProvider(httpClient);
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
        public void GivenAService_ProviderCallsHealthCheckEndpoint()
        {
            _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            _handler
                .Requests
                .Should()
                .Contain(r => r.RequestUri.PathAndQuery == "/service/healthcheck");
        }

        [Fact]
        public void GivenOkResponseWithEmptyContent_ResultIsEmptySet()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck")
                .With(HttpStatusCode.OK)
                .AndContent("application/json", "");

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenServiceUnavailableResponse_ResultIsEmptySet()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck")
                .With(HttpStatusCode.ServiceUnavailable)
                .AndContent("application/json", "");

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenOkResponseWithOneHealthyCheck_ResultContainsOneHealthyDependency()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck")
                .With(HttpStatusCode.OK)
                .AndContent("application/json", "{\"tests\":[{\"test_name\":\"foo\",\"test_result\":\"passed\"}]}");

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Single()
                .Health
                .Should()
                .Be(ServiceHealth.Healthy);
        }

        [Fact]
        public void GivenOkResponseWithOneUnhealthyCheck_ResultContainsOneUnhealthyDependency()
        {
            _handler
                .RespondTo(HttpMethod.Get, "/service/healthcheck")
                .With(HttpStatusCode.OK)
                .AndContent("application/json", "{\"tests\":[{\"test_name\":\"foo\",\"test_result\":\"failed\"}]}");

            var result = _provider.ProvideFor(new Service
            {
                Location = new Uri("https://tempuri.org")
            });

            result
                .Single()
                .Health
                .Should()
                .Be(ServiceHealth.Unhealthy);
        }
    }
}