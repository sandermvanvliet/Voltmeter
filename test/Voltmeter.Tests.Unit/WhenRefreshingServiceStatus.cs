using System;
using System.Linq;
using FluentAssertions;
using Moq;
using Voltmeter.Exceptions;
using Voltmeter.Ports.Providers;
using Voltmeter.UseCases;
using Xunit;

namespace Voltmeter.Tests.Unit
{
    public class WhenRefreshingServiceStatus
    {
        private readonly RefreshServiceStatusUseCase _useCase;
        private readonly Mock<IServiceStatusProvider> _statusProviderMock;
        private readonly Mock<IServiceDependenciesProvider> _dependenciesProviderMock;
        private readonly Service _service;

        public WhenRefreshingServiceStatus()
        {
            _statusProviderMock = new Mock<IServiceStatusProvider>();
            _dependenciesProviderMock = new Mock<IServiceDependenciesProvider>();

            _useCase = new RefreshServiceStatusUseCase(
                _statusProviderMock.Object, 
                _dependenciesProviderMock.Object);

            _service = new Service
            {
                Environment = new Environment {Name = "test"},
                Location = new Uri("https://tempuri.org/"),
                Name = "test service"
            };
        }

        [Fact]
        public void GivenServiceIsNull_ArgumentNullExceptionIsThrown()
        {
            Action action = () => _useCase.Refresh(null);

            action
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenServiceStatusIsHealthy_RefreshedStatusIsHealthy()
        {
            GivenServiceIsHealthy();

            var status = WhenRefreshingStatus();

            status
                .IsHealthy
                .Should()
                .BeTrue();
        }

        [Fact]
        public void GivenServiceDoesNotExist_ServiceNotFoundExceptionIsThrown()
        {
            GivenServiceDoesNotExist();

            Action action = () => WhenRefreshingStatus();

            action
                .Should()
                .Throw<ServiceNotFoundException>();
        }

        [Fact]
        public void GivenServiceHasTwoDependencies_DependenciesAreInResult()
        {
            GivenServiceIsHealthy();
            GivenServiceHasNumberOfDependencies(2);

            var result = WhenRefreshingStatus();

            result
                .Dependencies
                .Should()
                .HaveCount(2);
        }

        private void GivenServiceHasNumberOfDependencies(int number)
        {
            _dependenciesProviderMock
                .Setup(s => s.ProvideFor(It.IsService(_service)))
                .Returns<Service>(
                    service =>
                        Enumerable
                            .Range(1, number)
                            .Select(n => new DependencyStatus())
                            .ToArray());
        }

        private void GivenServiceDoesNotExist()
        {
            _statusProviderMock
                .Setup(s => s.ProvideFor(It.IsService(_service)))
                .Callback<Service>(service => throw new ServiceNotFoundException(service));
        }

        private void GivenServiceIsHealthy()
        {
            _statusProviderMock
                .Setup(s => s.ProvideFor(It.IsService(_service)))
                .Returns<Service>(ServiceStatus.HealthyFrom);

        }

        private ServiceStatus WhenRefreshingStatus()
        {
            return _useCase.Refresh(_service);
        }
    }
}