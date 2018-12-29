using System;
using FluentAssertions;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Serilog.Sinks.InMemory.Assertions;
using Xunit;

namespace Voltmeter.Tests.Unit
{
    public class WhenRefreshingEnvironmentStatus
    {
        private const string EnvironmentName = "some environment";
        private readonly RefreshEnvironmentStatusUseCase _useCase;
        private readonly Mock<IEnvironmentStatusProvider> _providerMock;
        private readonly Mock<IEnvironmentStatusStore> _retrieverMock;

        public WhenRefreshingEnvironmentStatus()
        {
            _providerMock = new Mock<IEnvironmentStatusProvider>();
            _retrieverMock = new Mock<IEnvironmentStatusStore>();

            var logger = new LoggerConfiguration()
                .WriteTo.InMemory()
                .CreateLogger();

            _useCase = new RefreshEnvironmentStatusUseCase(_providerMock.Object, _retrieverMock.Object, logger);
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
            _providerMock
                .Setup(p => p.ProvideFor(It.Is<string>(e => e == EnvironmentName)))
                .Returns(new ApplicationStatus[0]);

            _useCase.Refresh(EnvironmentName);

            _retrieverMock
                .Verify(
                    r => r.Update(It.Is<string>(e => e == EnvironmentName), It.IsAny<ApplicationStatus[]>()),
                    Times.Never);
        }

        [Fact]
        public void GivenProviderRThrowsException_RetrieverIsNotUpdated()
        {
            _providerMock
                .Setup(p => p.ProvideFor(It.Is<string>(e => e == EnvironmentName)))
                .Throws(new Exception("BANG!"));
                
            _useCase.Refresh(EnvironmentName);

            _retrieverMock
                .Verify(
                    r => r.Update(It.Is<string>(e => e == EnvironmentName), It.IsAny<ApplicationStatus[]>()),
                    Times.Never);
        }

        [Fact]
        public void GivenProviderRThrowsException_ErrorIsLogged()
        {
            _providerMock
                .Setup(p => p.ProvideFor(It.Is<string>(e => e == EnvironmentName)))
                .Throws(new Exception("BANG!"));

            _useCase.Refresh(EnvironmentName);

            InMemorySink
                .Instance
                .Should()
                .HaveMessage("Could not get status of {Environment}")
                .Appearing().Once()
                .WithLevel(LogEventLevel.Error)
                .WithProperty("Environment")
                .WithValue(EnvironmentName);
        }

        [Fact]
        public void GivenProviderReturnsResults_RetrieverIsUpdated()
        {
            _providerMock
                .Setup(p => p.ProvideFor(It.Is<string>(e => e == EnvironmentName)))
                .Returns(new [] { new ApplicationStatus() });

            _useCase.Refresh(EnvironmentName);

            _retrieverMock
                .Verify(
                    r => r.Update(It.Is<string>(e => e == EnvironmentName), It.IsAny<ApplicationStatus[]>()),
                    Times.Once);
        }
    }
}