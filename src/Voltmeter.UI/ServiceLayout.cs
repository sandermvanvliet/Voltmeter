using System.Collections.Generic;
using System.Linq;
using Voltmeter.UI.Models;

namespace Voltmeter.UI
{
    public class ServiceLayout
    {
        public static void ApplyTo(ServiceModel[] services, IEnumerable<DependencyModel> edges)
        {
            // Simple approach first: order nodes based on number of inbound dependencies
            foreach (var service in services)
            {
                service.Level = edges.Count(e => e.To == service.Id);
            }

            while (true)
            {
                var changesMade = false;

                foreach (var service in services.OrderBy(s => s.Level))
                {
                    var outboundDependencies = edges
                        .Where(e => e.From == service.Id && e.To != service.Id)
                        .ToArray();

                    foreach (var dependency in outboundDependencies)
                    {
                        var dependentService = services.Single(s => s.Id == dependency.To);

                        if (dependentService.Level <= service.Level)
                        {
                            dependentService.Level = service.Level + 1;
                            changesMade = true;
                        }
                    }
                }

                if (!changesMade)
                {
                    break;
                }
            }
        }
    }
}