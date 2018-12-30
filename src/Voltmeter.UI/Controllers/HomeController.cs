using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Voltmeter.Ports.Storage;
using Voltmeter.UI.Models;

namespace Voltmeter.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEnvironmentStatusStore _store;
        private readonly string _defaultEnvironmentName;

        public HomeController(VoltmeterSettings settings, IEnvironmentStatusStore store)
        {
            _store = store;
            _defaultEnvironmentName = settings.DefaultEnvironmentName;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(string environmentName)
        {
            if (environmentName == null)
            {
                environmentName = _defaultEnvironmentName;
            }

            var applicationStatuses = _store.GetFor(environmentName);

            var serviceModels = ServiceModel.FromStatuses(applicationStatuses);

            var result = EdgeGenerator.For(serviceModels);

            var services = serviceModels.Concat(result.CreatedServices).ToArray();

            ServiceLayout.ApplyTo(services, result.Edges);

            var model = new EnvironmentStatusModel
            {
                Environment = environmentName,
                Services = services,
                Edges = result.Edges.ToArray(),
                AvailableEnvironments = _store.GetAvailableEnvironments()
            };

            return View(model);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
