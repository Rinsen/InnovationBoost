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
        private readonly RandomStringGenerator _randomStringGenerator;

        public HomeController(IdentityServerClientBusiness identityServerClientBusiness,
            RandomStringGenerator randomStringGenerator)
        {
            _identityServerClientBusiness = identityServerClientBusiness;
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

                client = await _identityServerClientBusiness.GetIdentityServerClient("testclient");
            }

            

            ViewBag.Data = JsonConvert.SerializeObject(client); ;

            //var secret = _randomStringGenerator.GetRandomString(60);

            //disconnectedClientGraph.ClientSecrets.Add(new IdentityServerClientSecret
            //{
            //    Expiration = null,
            //    State = ObjectState.Added,
            //    Type = IdentityServer4.IdentityServerConstants.SecretTypes.SharedSecret,
            //    Value = secret.Sha256()
            //});

            //await _identityServerClientBusiness.UpdateClient(disconnectedClientGraph);

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
