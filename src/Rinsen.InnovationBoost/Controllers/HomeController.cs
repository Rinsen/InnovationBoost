using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Rinsen.InnovationBoost.Controllers
{
    public class HomeController : Controller
    {

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    logger.LogInformation("Helloooo");
        //}

        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult TestErrorHandling()
        {
            throw new System.Exception("This is a test exception");
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }
    }
}
