using System.Linq;
using FluentAssertions;
using Voltmeter.UI.Models;
using Xunit;

namespace Voltmeter.UI.Tests.Unit
{
    public class WhenMappingServiceStatusToApplicationModel
    {
        [Fact]
        public void GivenListOfStatuses_ModelsHaveSequentialIds()
        {
            var statuses = new[]
            {
                new ServiceStatus(),
                new ServiceStatus(),
                new ServiceStatus(),
                new ServiceStatus(),
            };

            var models = ApplicationModel.FromStatuses(statuses);

            models
                .Select(m => m.Id)
                .Should()
                .ContainInOrder(1, 2, 3, 4);
        }

        [Fact]
        public void GivenStatus_ModelHasApplicationName()
        {
            var status = new ServiceStatus {Name = "appOne"};

            ApplicationModel
                .FromStatus(status)
                .Name
                .Should()
                .Be("appOne");
        }

        [Fact]
        public void GivenStatusWithHealthyState_ModelHasColorGreen()
        {
            var status = new ServiceStatus { Name = "appOne", IsHealthy = true };

            ApplicationModel
                .FromStatus(status)
                .Color
                .Should()
                .Be("#00ff00");
        }
    }
}