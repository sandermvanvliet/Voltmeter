using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Voltmeter.Ports.Storage;
using Voltmeter.UI.Controllers;
using Voltmeter.UI.Models;
using Xunit;

namespace Voltmeter.UI.Tests.Unit
{
    public class WhenDisplayingForEnvironment
    {
        private readonly HomeController _controller;
        private readonly Mock<IEnvironmentStatusStore> _statusRetrieverMock;
        private readonly List<string> _availableEnvironments;

        public WhenDisplayingForEnvironment()
        {
            _availableEnvironments = new List<string>();

            _statusRetrieverMock = new Mock<IEnvironmentStatusStore>();
            _statusRetrieverMock
                .Setup(s => s.GetAvailableEnvironments())
                .Returns(() => _availableEnvironments.ToArray());

            _controller = new HomeController(new VoltmeterSettings { DefaultEnvironmentName = "defaultEnv" }, _statusRetrieverMock.Object);
        }

        private const string EnvironmentName = "production";

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_EnvironmentStatusModelIsReturned()
        {
            GivenDetailsFor(EnvironmentName);

            var result = WhenDisplayingEnvironment();

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>();
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_ModelContainsDetails()
        {
            GivenDetailsFor(EnvironmentName);

            var result = WhenDisplayingEnvironment();

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Services
                .Should()
                .NotBeEmpty();
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_ModelContainsEnvironmentName()
        {
            GivenDetailsFor(EnvironmentName);

            var result = WhenDisplayingEnvironment();

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Environment
                .Should()
                .Be(EnvironmentName);
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_EdgesContainsAllDependencies()
        {
            GivenDetailsFor(EnvironmentName);

            var result = WhenDisplayingEnvironment();

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Edges
                .Should()
                .HaveCount(2);
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsNull_ModelContainsDefaultEnvironmentName()
        {
            GivenDetailsFor(EnvironmentName);
            
            var result = (ViewResult)_controller.Index(null);

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Environment
                .Should()
                .Be("defaultEnv");
        }

        [Fact]
        public void GivenFourEnvironmentsExist_ModelContainsListOfThoseEnvironments()
        {
            GivenDetailsFor("one");
            GivenDetailsFor("two");
            GivenDetailsFor("three");
            GivenDetailsFor("four");
            
            var result = (ViewResult)_controller.Index(null);

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .AvailableEnvironments
                .Should()
                .Contain("one", "two", "three", "four");
        }

        private void GivenDetailsFor(string environmentName)
        {
            _statusRetrieverMock
                .Setup(s => s.GetFor(It.Is<string>(e => e == environmentName)))
                .Returns(new[]
                {
                    new ServiceStatus
                    {
                        Dependencies = new[]
                        {
                            new DependencyStatus(),
                            new DependencyStatus()
                        }
                    }
                });

            _availableEnvironments.Add(environmentName);
        }

        private ViewResult WhenDisplayingEnvironment()
        {
            return _controller.Index(EnvironmentName) as ViewResult;
        }
    }
}
