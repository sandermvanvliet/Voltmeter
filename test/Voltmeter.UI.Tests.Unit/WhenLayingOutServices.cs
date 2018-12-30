using FluentAssertions;
using Voltmeter.UI.Models;
using Xunit;

namespace Voltmeter.UI.Tests.Unit
{
    public class WhenLayingOutServices
    {
        [Fact]
        public void GivenServiceOneDependsOnServiceTwo_LevelOfOneIsHigherThanTwo()
        {
            var serviceOne = new ServiceModel {Id = 1, Name = "one", Level = 0};
            var serviceTwo = new ServiceModel {Id = 2, Name = "two", Level = 0};

            var services = new[] {serviceOne, serviceTwo};
            var edges = new[] {new DependencyModel {From = serviceOne.Id, To = serviceTwo.Id}};

            ServiceLayout.ApplyTo(services, edges);

            serviceOne
                .Level
                .Should()
                .BeLessThan(serviceTwo.Level);
        }

        [Fact]
        public void GivenBothServicesDependOnThird_OneAndTwoAreAboveThree()
        {
            var serviceOne = new ServiceModel {Id = 1, Name = "one", Level = 0};
            var serviceTwo = new ServiceModel {Id = 2, Name = "two", Level = 0};
            var serviceThree = new ServiceModel {Id = 3, Name = "three", Level = 0};

            var services = new[] {serviceOne, serviceTwo, serviceThree};
            var edges = new[]
            {
                new DependencyModel {From = serviceOne.Id, To = serviceThree.Id},
                new DependencyModel {From = serviceTwo.Id, To = serviceThree.Id}
            };

            ServiceLayout.ApplyTo(services, edges);

            serviceThree
                .Level
                .Should()
                .BeGreaterThan(serviceOne.Level)
                .And
                .BeGreaterThan(serviceTwo.Level);
        }

        [Fact]
        public void GivenOneDepdensOnTwoWhichDependsOnThree_OneIsAboveTwoIsAboveThree()
        {
            var serviceOne = new ServiceModel { Id = 1, Name = "one", Level = 0 };
            var serviceTwo = new ServiceModel { Id = 2, Name = "two", Level = 0 };
            var serviceThree = new ServiceModel { Id = 3, Name = "three", Level = 0 };

            var services = new[] { serviceOne, serviceTwo, serviceThree };
            var edges = new[]
            {
                new DependencyModel {From = serviceOne.Id, To = serviceTwo.Id},
                new DependencyModel {From = serviceTwo.Id, To = serviceThree.Id}
            };

            ServiceLayout.ApplyTo(services, edges);

            serviceThree
                .Level
                .Should()
                .BeGreaterThan(serviceOne.Level)
                .And
                .BeGreaterThan(serviceTwo.Level);

            serviceTwo
                .Level
                .Should()
                .BeGreaterThan(serviceOne.Level);
        }
    }
}