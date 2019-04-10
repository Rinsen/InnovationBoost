using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace LoginAndLogging.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        // GET: /<controller>/
        [AllowAnonymous]
        public IActionResult Index()
        {
            var id = 1;
            _logger.LogInformation("Display index {id}", id);

            return View();
        }

        public IActionResult Index2()
        {
            var id = 1;
            _logger.LogInformation("Logged in user {Name} with id {Id}", User.Identity.Name, id);

            return View();
        }

        [AllowAnonymous]
        public IActionResult Error()
        {
            return View();
        }

        [AllowAnonymous]
        // GET: /<controller>/
        public IActionResult GetError()
        {
            throw new NotImplementedException("Not done", new Exception("My inner exception"));
        }
    }
}
