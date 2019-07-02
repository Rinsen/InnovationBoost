using Microsoft.AspNetCore.Mvc;

namespace Rinsen.InnovationBoost.Controllers
{
    public class HomeController : Controller
    {

        public HomeController()
        {
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult TestErrorHandling()
        {
            throw new System.Exception("This is a test exception");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
