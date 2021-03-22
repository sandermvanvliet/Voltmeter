using System.Linq;

namespace Voltmeter.UI.Models
{
    public class ServiceModel : ServiceStatus
    {
        public int Id { get; set; }
        public int  Level { get; set; }
        public string Color { get; set; }
        public string Info => $"{Name}<br/>{Location}";
        public bool IsExternal { get; set; }

        public static ServiceModel[] FromStatuses(ServiceStatus[] statuses)
        {
            return statuses
                .Select(FromStatus)
                .Select((model, index) =>
                {
                    model.Id = index + 1; // index is zero based and we want 1 based ids
                    return model;
                })
                .ToArray();
        }

        public static ServiceModel FromStatus(ServiceStatus serviceStatus)
        {
            return new ServiceModel
            {
                Environment = serviceStatus.Environment,
                Dependencies = serviceStatus.Dependencies,
                Health = serviceStatus.Health,
                Location = serviceStatus.Location,
                Name = serviceStatus.Name,
                Color = EdgeGenerator.ColorForHealthStatus(serviceStatus.Health)
            };
        }
    }
}