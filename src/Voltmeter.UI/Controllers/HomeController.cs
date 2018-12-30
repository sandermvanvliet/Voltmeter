using System.Diagnostics;
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

            var model = new EnvironmentStatusModel
            {
                Environment = environmentName,
                Services = ServiceModel.FromStatuses(applicationStatuses),
                Edges = new DependencyModel[0],
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
