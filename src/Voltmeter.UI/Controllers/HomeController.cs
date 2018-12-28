using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Voltmeter.UI.Models;

namespace Voltmeter.UI.Controllers
{
    public class HomeController : Controller
    {
        private readonly IEnvironmentStatusRetriever _retriever;
        private readonly string _defaultEnvironmentName;

        public HomeController(VoltmeterSettings settings, IEnvironmentStatusRetriever retriever)
        {
            _retriever = retriever;
            _defaultEnvironmentName = settings.DefaultEnvironmentName;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(string environmentName)
        {
            if (environmentName == null)
            {
                environmentName = _defaultEnvironmentName;
            }

            var applicationStatuses = _retriever.GetFor(environmentName);

            var model = new EnvironmentStatusModel
            {
                Environment = environmentName,
                Applications = ApplicationModel.FromStatuses(applicationStatuses),
                Edges = new DependencyModel[0]
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
