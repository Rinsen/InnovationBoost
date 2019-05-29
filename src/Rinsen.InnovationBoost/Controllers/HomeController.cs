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
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;
        private readonly RandomStringGenerator _randomStringGenerator;

        public HomeController(IdentityServerClientBusiness identityServerClientBusiness,
            IdentityServerApiResourceBusiness identityServerApiResourceBusiness,
            IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness,
            RandomStringGenerator randomStringGenerator)
        {
            _identityServerClientBusiness = identityServerClientBusiness;
            _identityServerApiResourceBusiness = identityServerApiResourceBusiness;
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
            _randomStringGenerator = randomStringGenerator;
        }

        [AllowAnonymous]
        [HttpGet]
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
