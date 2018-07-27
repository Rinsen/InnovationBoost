using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace LoginAndLogging.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            var id = 1;
            _logger.LogDebug("Display index {id}", id);

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }

        // GET: /<controller>/
        public IActionResult GetError()
        {
            throw new NotImplementedException("Not done", new Exception("My inner exception"));
        }
    }
}
