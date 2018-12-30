using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using Voltmeter.Ports.Discovery;
using Voltmeter.Ports.Providers;
using Voltmeter.Ports.Storage;
using Voltmeter.UseCases;
using Xunit;

namespace Voltmeter.Tests.Unit
{
    public class WhenRefreshingEnvironmentStatus
    {
        private const string EnvironmentName = "some environment";
        private readonly RefreshEnvironmentStatusUseCase _useCase;
        private readonly Mock<IEnvironmentStatusStore> _storeMock;
        private readonly Mock<IServiceDiscovery> _serviceDiscoveryMock;
        private readonly Mock<IServiceStatusProvider> _serviceStatusMock;

        public WhenRefreshingEnvironmentStatus()
        {
            _storeMock = new Mock<IEnvironmentStatusStore>();
            _serviceDiscoveryMock = new Mock<IServiceDiscovery>();

            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            _serviceStatusMock = new Mock<IServiceStatusProvider>();
            _useCase = new RefreshEnvironmentStatusUseCase(
                _storeMock.Object, 
                logger,
                _serviceDiscoveryMock.Object,
                new RefreshServiceStatusUseCase(
                    _serviceStatusMock.Object, 
                    new Mock<IServiceDependenciesProvider>().Object));
        }

        [Fact]
        public void GivenEnvironmentNameIsEmpty_ArgumentNullExceptionIsThrown()
        {
            Action action = () => _useCase.Refresh(null);

            action
                .Should()
                .Throw<ArgumentNullException>();
        }

        [Fact]
        public void GivenProviderReturnsEmptyResultSet_RetrieverIsNotUpdated()
        {
            _serviceDiscoveryMock
                .Setup(s => s.DiscoverServicesIn(It.IsEnvironment(EnvironmentName)))
                .Returns<Environment>(e => new Service[0]);

            WhenRefreshing();

            _storeMock
                .Verify(
                    r => r.Update(It.IsEnvironment(EnvironmentName), Moq.It.IsAny<ServiceStatus[]>()),
                    Times.Never);
        }

        [Fact]
        public void GivenProviderRThrowsException_RetrieverIsNotUpdated()
        {
            _serviceDiscoveryMock
                .Setup(s => s.DiscoverServicesIn(It.IsEnvironment(EnvironmentName)))
                .Throws(new Exception("BANG!"));
                
            WhenRefreshing();

            _storeMock
                .Verify(
                    r => r.Update(It.IsEnvironment(EnvironmentName), Moq.It.IsAny<ServiceStatus[]>()),
                    Times.Never);
        }

        [Fact]
        public void GivenProviderRThrowsException_ErrorIsLogged()
        {
            _serviceDiscoveryMock
                .Setup(s => s.DiscoverServicesIn(It.IsEnvironment(EnvironmentName)))
                .Throws(new Exception("BANG!"));

            WhenRefreshing();

            InMemorySink
                .Instance
                .Should()
                .HaveMessage("Could not get status of {Environment}")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Error)
                .WithProperty("Environment")
                .WithValue(new Environment().ToString());
        }

        [Fact]
        public void GivenProviderReturnsResults_RetrieverIsUpdatedWithService()
        {
            var serviceName = "SOME SERVICE";

            _serviceDiscoveryMock
                .Setup(s => s.DiscoverServicesIn(It.IsEnvironment(EnvironmentName)))
                .Returns<Environment>(e => new[] {new Service {Environment = e, Name = serviceName } });

            _serviceStatusMock
                .Setup(s => s.ProvideFor(It.IsService(serviceName)))
                .Returns<Service>(ServiceStatus.HealthyFrom);

            WhenRefreshing();

            _storeMock
                .Verify(
                    r => r.Update(
                        It.IsEnvironment(EnvironmentName),
                        Moq.It.Is<IEnumerable<ServiceStatus>>(services => services.Any(s => s.Name == serviceName))),
                    Times.Once);
        }

        private void WhenRefreshing()
        {
            _useCase.Refresh(new Environment { Name = EnvironmentName });
        }
    }
}