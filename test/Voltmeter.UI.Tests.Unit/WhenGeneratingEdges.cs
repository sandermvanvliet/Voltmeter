using FluentAssertions;
using Voltmeter.UI.Models;
using Xunit;

namespace Voltmeter.UI.Tests.Unit
{
    public class WhenGeneratingEdges
    {
        [Fact]
        public void GivenTwoServicesWithoutDependencies_NoEdgesAreGenerated()
        {
            var services = new[]
            {
                new ServiceModel {Name = "one"},
                new ServiceModel {Name = "two"}
            };

            var result = EdgeGenerator.For(services);

            result
                .Edges
                .Should()
                .BeEmpty();
        }

        [Fact]
        public void GivenServiceOneDependsOnServiceTwo_OneEdgeIsGenerated()
        {
            var serviceOne = new ServiceModel {Name = "one", Dependencies = new []
            {
                new DependencyStatus{ IsHealthy = true, Name = "two" }
            }};
            var serviceTwo = new ServiceModel { Name = "two", Id = 2 };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .Edges
                .Should()
                .HaveCount(1)
                .And
                .Contain(edge => edge.From == serviceOne.Id && edge.To == serviceTwo.Id);
        }

        [Fact]
        public void GivenServiceOneDependsOnNonExistingService_OneEdgeIsGenerated()
        {
            var serviceOne = new ServiceModel
            {
                Name = "one",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                }
            };
            var serviceTwo = new ServiceModel { Name = "two", Id = 2 };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .Edges
                .Should()
                .HaveCount(1)
                .And
                .Contain(edge => edge.From == serviceOne.Id && edge.To == 3);
        }

        [Fact]
        public void GivenServiceOneDependsOnNonExistingService_NewServiceModelIsAdded()
        {
            var serviceOne = new ServiceModel
            {
                Name = "one",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                }
            };
            var serviceTwo = new ServiceModel { Name = "two", Id = 2 };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .CreatedServices
                .Should()
                .Contain(edge => edge.Name == "some other service");
        }

        [Fact]
        public void GivenServiceOneDependsOnNonExistingService_NewServiceHasSequentialId()
        {
            var serviceOne = new ServiceModel
            {
                Name = "one",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                }
            };
            var serviceTwo = new ServiceModel { Name = "two", Id = 2 };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .CreatedServices
                .Should()
                .ContainSingle(edge => edge.Name == "some other service")
                .Which
                .Id
                .Should()
                .Be(3);
        }

        [Fact]
        public void GivenServiceOneAndTwoDependOnNonExistingService_OneNewServiceModelIsAdded()
        {
            var serviceOne = new ServiceModel
            {
                Name = "one",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                },
                Id = 1
            };
            var serviceTwo = new ServiceModel { Name = "two",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                },
                Id = 2
            };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .CreatedServices
                .Should()
                .ContainSingle(edge => edge.Name == "some other service");
        }

        [Fact]
        public void GivenServiceOneAndTwoDependOnNonExistingService_TwoEdgesAreCreatedToNewService()
        {
            var serviceOne = new ServiceModel
            {
                Name = "one",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                },
                Id = 1
            };
            var serviceTwo = new ServiceModel
            {
                Name = "two",
                Dependencies = new[]
                {
                    new DependencyStatus{ IsHealthy = true, Name = "some other service" }
                },
                Id = 2
            };

            var services = new[]
            {
                serviceOne,
                serviceTwo
            };

            var result = EdgeGenerator.For(services);

            result
                .Edges
                .Should()
                .ContainSingle(edge => edge.From == 1 && edge.To == 3)
                .And
                .ContainSingle(edge => edge.From == 2 && edge.To == 3);
        }
    }
}