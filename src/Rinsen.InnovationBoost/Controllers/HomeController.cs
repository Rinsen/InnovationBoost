using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.IdentityServer;

namespace Rinsen.InnovationBoost.Controllers
{
    public class HomeController : Controller
    {
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;

        public HomeController(IdentityServerClientBusiness identityServerClientBusiness)
        {
            _identityServerClientBusiness = identityServerClientBusiness;

            
        }
        //public HomeController(ILogger<HomeController> logger)
        //{
        //    logger.LogInformation("Helloooo");
        //}

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var client = await _identityServerClientBusiness.GetClient("mvc");

            if (client == default)
            {
                await _identityServerClientBusiness.CreateTestData();

                client = await _identityServerClientBusiness.GetClient("mvc");
            }

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
