using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Voltmeter.UI.Controllers;
using Voltmeter.UI.Models;
using Xunit;

namespace Voltmeter.UI.Tests.Unit
{
    public class WhenDisplayingForEnvironment
    {
        private readonly HomeController _controller;

        public WhenDisplayingForEnvironment()
        {
            _controller = new HomeController(new VoltmeterSettings { DefaultEnvironmentName = "defaultEnv" });
        }

        private const string EnvironmentName = "production";

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_EnvironmentStatusModelIsReturned()
        {
            GivenDetailsFor(EnvironmentName);

            var result = _controller.Index(EnvironmentName) as ViewResult;

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>();
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_ModelContainsDetails()
        {
            GivenDetailsFor(EnvironmentName);

            var result = _controller.Index(EnvironmentName) as ViewResult;

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Applications
                .Should()
                .NotBeEmpty();
        }

        [Fact]
        public void GivenDetailsForProductionAndEnvironmentNameIsProduction_ModelContainsEnvironmentName()
        {
            GivenDetailsFor(EnvironmentName);

            var result = _controller.Index(EnvironmentName) as ViewResult;

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
        public void GivenDetailsForProductionAndEnvironmentNameIsNull_ModelContainsDefaultEnvironmentName()
        {
            GivenDetailsFor(EnvironmentName);

            var result = _controller.Index(null) as ViewResult;

            result
                .Model
                .Should()
                .BeOfType<EnvironmentStatusModel>()
                .Which
                .Environment
                .Should()
                .Be("defaultEnv");
        }

        private void GivenDetailsFor(string environmentName)
        {
        }
    }
}
