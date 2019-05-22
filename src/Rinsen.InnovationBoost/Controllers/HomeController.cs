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
        public async Task<IActionResult> Index()
        {
            var client = await _identityServerClientBusiness.GetIdentityServerClient("testclient");

            if (client == default)
            {
                await _identityServerClientBusiness.CreateNewClient("testclient", "Test Client", "Client for testing EF");
            }

            var apiResource = await _identityServerApiResourceBusiness.GetApiResourceAsync("testresource");

            if (apiResource == default)
            {
                await _identityServerApiResourceBusiness.CreateNewApiResource("testapiresource", "Test Api Resource", "Api Resource for testing EF");
            }

            var identityResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync("testapiresource");

            if (identityResource == default)
            {
                await _identityServerIdentityResourceBusiness.CreateNewIdentityResourceAsync("testidentityresource", "Test Identity Resource", "Identity Resource for testing EF");
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Index(string data)
        {
            
            var disconnectedClientGraph = JsonConvert.DeserializeObject<IdentityServerClient>(data);

            //var secret = _randomStringGenerator.GetRandomString(60);

            //disconnectedClientGraph.ClientSecrets.Add(new IdentityServerClientSecret
            //{
            //    Expiration = null,
            //    State = ObjectState.Added,
            //    Type = IdentityServer4.IdentityServerConstants.SecretTypes.SharedSecret,
            //    Value = secret.Sha256()
            //});

            disconnectedClientGraph.ClientSecrets.First().State = ObjectState.Deleted;

            await _identityServerClientBusiness.UpdateClient(disconnectedClientGraph);

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
