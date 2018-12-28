using System.Linq;

namespace Voltmeter.UI.Models
{
    public class ApplicationModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int  Level { get; set; }
        public string Color { get; set; }

        public static ApplicationModel[] FromStatuses(ApplicationStatus[] statuses)
        {
            return statuses
                .Select((s, index) =>
                {
                    var model = FromStatus(s);

                    model.Id = index + 1;

                    return model;
                })
                .ToArray();
        }

        public static ApplicationModel FromStatus(ApplicationStatus applicationStatus)
        {
            return new ApplicationModel
            {
                Name = applicationStatus.Name,
                Color = applicationStatus.IsHealthy ? "#00ff00" : "#ff0000"
            };
        }
    }
}