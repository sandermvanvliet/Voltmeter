using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Voltmeter.UI.Models;

namespace Voltmeter.UI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index(string environmentName = "production")
        {
            var model = new EnvironmentStatusModel
            {
                Applications = new []
                {
                    new ApplicationModel()
                }
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
