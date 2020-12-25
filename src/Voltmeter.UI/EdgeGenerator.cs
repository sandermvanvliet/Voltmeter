using System.Collections.Generic;
using System.Linq;
using Voltmeter.UI.Models;

namespace Voltmeter.UI
{
    public class EdgeGenerator
    {
        public static GeneratedEdges For(ServiceModel[] services)
        {
            var edges = new List<DependencyModel>();
            var createdServices = new List<ServiceModel>();

            foreach (var service in services)
            {
                if (service.Dependencies == null)
                {
                    continue;
                }

                foreach (var dependency in service.Dependencies)
                {
                    var match = services.SingleOrDefault(s => s.Name == dependency.Name)
                        ?? createdServices.SingleOrDefault(s => s.Name == dependency.Name);

                    if (match == null)
                    {
                        match = new ServiceModel
                        {
                            Id = services.Length + createdServices.Count + 1,
                            Name = dependency.Name,
                            Environment = service.Environment,
                            Color = ColorForHealthStatus(dependency.Health),
                            IsExternal = true,
                            Dependencies = new DependencyStatus[0],
                            Health = dependency.Health
                        };

                        createdServices.Add(match);
                    }

                    var edge = new DependencyModel
                    {
                        From = service.Id,
                        To = match.Id,
                        Color = ColorForHealthStatus(dependency.Health)
                    };

                    edges.Add(edge);
                }
            }

            return new GeneratedEdges
            {
                Edges = edges,
                CreatedServices = createdServices
            };
        }

        public static string ColorForHealthStatus(ServiceHealth dependencyHealth)
        {
            switch (dependencyHealth)
            {
                case ServiceHealth.Unknown:
                    return "#cccccc";
                case ServiceHealth.Healthy:
                    return "#00ff00";
                case ServiceHealth.Degraded:
                    return "##ff7f00";
                default:
                    return "#ff0000";
            }
        }
    }

    public class GeneratedEdges
    {
        public IEnumerable<DependencyModel> Edges { get; set; }
        public IEnumerable<ServiceModel> CreatedServices { get; set; }
    }
}