using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.IdentityServer;
using Rinsen.InnovationBoost.Models;

namespace Rinsen.InnovationBoost.Controllers
{
    public class DeviceController : Controller
    {
        private readonly IDeviceFlowInteractionService _deviceFlowInteractionService;
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;
        private readonly IResourceStore _resourceStore;
        private readonly ILogger<DeviceController> _logger;

        public DeviceController(IDeviceFlowInteractionService deviceFlowInteractionService,
            IdentityServerClientBusiness identityServerClientBusiness,
            IResourceStore resourceStore,
            ILogger<DeviceController> logger
            )
        {
            _deviceFlowInteractionService = deviceFlowInteractionService;
            _identityServerClientBusiness = identityServerClientBusiness;
            _resourceStore = resourceStore;
            _logger = logger;
            // https://github.com/IdentityServer/IdentityServer4.Demo/blob/master/src/IdentityServer4Demo/Quickstart/Device/DeviceController.cs
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromQuery(Name = "user_code")] string userCode)
        {
            var deviceConcentModel = new DeviceConsentModel();

            if (string.IsNullOrWhiteSpace(userCode))
                return View(deviceConcentModel);

            deviceConcentModel.UserCode = userCode;

            var request = await _deviceFlowInteractionService.GetAuthorizationContextAsync(userCode);

            if (request == null)
            {
                _logger.LogError("Request not found for user code: {0}", userCode);

                return View("Error");
            }

            var client = await _identityServerClientBusiness.GetIdentityServerClient(request.ClientId);

            if (client == null)
            {
                _logger.LogError("Invalid client id: {0}", request.ClientId);

                return View("Error");
            }

            deviceConcentModel.ClientName = client.ClientName ?? client.ClientId;
            deviceConcentModel.ClientUrl = client.ClientUri;
            deviceConcentModel.ClientLogoUrl = client.LogoUri;
            deviceConcentModel.AllowRememberConsent = client.AllowRememberConsent;

            var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ScopesRequested);

            if (resources == null || (!resources.IdentityResources.Any() && !resources.ApiResources.Any()))
            {
                _logger.LogError("No scopes matching: {0} for client {2}", request.ScopesRequested.Aggregate((x, y) => x + ", " + y), client.ClientId);

                return View("Error");
            }

            if (resources.IdentityResources.Any())
            {
                foreach (var scope in request.ScopesRequested)
                {
                    var identityResource = resources.IdentityResources.FirstOrDefault(m => m.Name == scope);

                    if (identityResource != default)
                    {
                        deviceConcentModel.IdentityScope.Add(identityResource);
                    }
                }
            }

            if (resources.ApiResources.Any())
            {
                foreach (var scope in request.ScopesRequested)
                {
                    foreach (var apiResource in resources.ApiResources)
                    {
                        var apiResourceScope = apiResource.Scopes.FirstOrDefault(m => m.Name == scope);

                        if (apiResourceScope != default)
                        {
                            deviceConcentModel.ResourceScope.Add(apiResourceScope);
                        }
                    }
                }
            }

            return View(deviceConcentModel);
        }

        [HttpPost]
        public IActionResult Concent(DeviceConcentModel deviceConcentModel)
        {



            return View();
        }

    }
}
