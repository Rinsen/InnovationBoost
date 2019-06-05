using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.IdentityProvider.IdentityServer.Entities;

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
