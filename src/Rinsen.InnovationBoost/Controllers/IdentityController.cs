using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.ExternalApplications;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.InnovationBoost.Models;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IExternalApplicationService _externalApplicationService;
        private readonly IIdentityService _identityService;
        private readonly ILocalAccountService _localAccountService;

        public IdentityController(ILoginService loginService,
            IExternalApplicationService externalApplicationService,
            IIdentityService identityService,
            ILocalAccountService localAccountService)
        {
            _loginService = loginService;
            _externalApplicationService = externalApplicationService;
            _identityService = identityService;
            _localAccountService = localAccountService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string externalUrl, string host, string applicationName)
        {
            var model = new LoginModel { ExternalUrl = externalUrl, Host = host, ApplicationName = applicationName };

            if (User.Identity.IsAuthenticated)
            {
                model.RedirectUrl = await RedirectToLocalOrTrustedExternalHostOnlyAsync(applicationName, externalUrl, host);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _loginService.LoginAsync(model.Email, model.Password, model.RememberMe);

                if (result.Succeeded)
                {
                    // Set loged in user to the one just created as this only will be provided at next request by the framework
                    HttpContext.User = result.Principal;

                    model.RedirectUrl = await RedirectToLocalOrTrustedExternalHostOnlyAsync(model.ApplicationName, model.ExternalUrl, model.Host);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }

            }

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {



            return View(new CreateIdentityModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> Create(CreateIdentityModel model)
        {
            if (ModelState.IsValid)
            {
                var createIdentityResult = await _identityService.CreateAsync(model.GivenName, model.Surname, model.Email, model.PhoneNumber);

                if (createIdentityResult.Succeeded)
                {
                    var createLocalAccountResult = await _localAccountService.CreateAsync(createIdentityResult.Identity.IdentityId, model.Email, model.Password);

                    if (createLocalAccountResult.Succeeded)
                    {
                        var loginResult = await _loginService.LoginAsync(model.Email, model.Password, false);

                        if (loginResult.Succeeded)
                        {
                            model.RedirectUrl = await RedirectToLocalOrTrustedExternalHostOnlyAsync(model.ApplicationName, model.ExternalUrl, model.Host);
                        }
                    }
                }

                ModelState.AddModelError(string.Empty, "Create user invalid.");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Logout()
        {



            return View();
        }

        private async Task<string> RedirectToLocalOrTrustedExternalHostOnlyAsync(string applicationName, string externalUrl, string host)
        {
            if (!string.IsNullOrEmpty(host))
            {
                var identityId = User.GetClaimGuidValue(ClaimTypes.NameIdentifier);
                var correlationId = User.GetClaimGuidValue(ClaimTypes.SerialNumber);
                var rememberMe = User.GetClaimBoolValue(ClaimTypes.Expiration);

                var result = await _externalApplicationService.GetTokenForValidHostAsync(applicationName, host, identityId, correlationId, rememberMe);

                if (result.Succeeded)
                {
                    // Always enforce https, no options on this
                    var uri = $"https://{host}{externalUrl}" + QueryString.Create("AuthToken", result.Token).ToUriComponent();

                    return uri;
                }

                throw new UnauthorizedAccessException($"External application is not found from Host {host}");
            }

            if (Url.IsLocalUrl(externalUrl))
            {
                return externalUrl;
            }

            return string.Empty;
        }
    }
}
