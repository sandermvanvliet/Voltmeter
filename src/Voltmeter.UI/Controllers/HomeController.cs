using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Voltmeter.UI.Models;

namespace Voltmeter.UI.Controllers
{
    public class HomeController : Controller
    {
        private const string DefaultEnvironmentName = "production";

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Index(string environmentName)
        {
            if (environmentName == null)
            {
                environmentName = DefaultEnvironmentName;
            }

            var model = new EnvironmentStatusModel
            {
                Environment = environmentName,
                Applications = new []
                {
                    new ApplicationModel()
                },
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
