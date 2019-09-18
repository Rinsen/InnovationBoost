using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.AuditLogging;
using Rinsen.IdentityProvider.Contracts;
using Rinsen.IdentityProvider.Contracts.v1;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.ExternalApplications;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.InnovationBoost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IIdentityAttributeStorage _identityAttributeStorage;
        private readonly AuditLog _auditLog;

        public IdentityController(ILoginService loginService,
            IExternalApplicationService externalApplicationService,
            IIdentityService identityService,
            ILocalAccountService localAccountService,
            IIdentityAttributeStorage identityAttributeStorage,
            AuditLog auditLog)
        {
            _loginService = loginService;
            _externalApplicationService = externalApplicationService;
            _identityService = identityService;
            _localAccountService = localAccountService;
            _identityAttributeStorage = identityAttributeStorage;
            _auditLog = auditLog;
        }

        [HttpGet] 
        [AllowAnonymous]
        public async Task<IActionResult> Login(string externalUrl, string host, string applicationName, string returnUrl)
        {
            var model = new LoginModel { ExternalUrl = externalUrl, Host = host, ApplicationName = applicationName, ReturnUrl = returnUrl };

            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    if (!Url.IsLocalUrl(model.ReturnUrl))
                    {
                        await _auditLog.Log("InvalidReturnUrl", $"Return Url '{model.ReturnUrl}'", HttpContext.Connection.RemoteIpAddress.ToString());

                        throw new UnauthorizedAccessException($"Only local redirects is allowed");
                    }

                    return Redirect(returnUrl);
                }

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
                var result = await _loginService.LoginAsync(model.Email, model.Password, Request.Host.Value, model.RememberMe);

                if (result.Succeeded)
                {
                    await _auditLog.Log("LoginSuccess", $"Email '{model.Email}'", HttpContext.Connection.RemoteIpAddress.ToString());

                    // Set loged in user to the one just created as this only will be provided at next request by the framework
                    HttpContext.User = result.Principal;

                    if (!string.IsNullOrEmpty(model.ReturnUrl))
                    {
                        if (!Url.IsLocalUrl(model.ReturnUrl))
                        {
                            await _auditLog.Log("InvalidReturnUrl", $"Email '{model.Email}', Return Url '{model.ReturnUrl}'", HttpContext.Connection.RemoteIpAddress.ToString());

                            throw new UnauthorizedAccessException($"Only local redirects is allowed");
                        }

                        return Redirect(model.ReturnUrl);
                    }

                    model.RedirectUrl = await RedirectToLocalOrTrustedExternalHostOnlyAsync(model.ApplicationName, model.ExternalUrl, model.Host);
                }
                else
                {
                    await _auditLog.Log("InvalidLoginAttempt", $"Email '{model.Email}'", HttpContext.Connection.RemoteIpAddress.ToString());

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
                    await _auditLog.Log("IdentityCreated", $"Email '{model.Email}', ", HttpContext.Connection.RemoteIpAddress.ToString());

                    var createLocalAccountResult = await _localAccountService.CreateAsync(createIdentityResult.Identity.IdentityId, model.Email, model.Password);

                    if (createLocalAccountResult.Succeeded)
                    {
                        var loginResult = await _loginService.LoginAsync(model.Email, model.Password, Request.Host.Value, false);

                        if (loginResult.Succeeded)
                        {
                            model.RedirectUrl = await RedirectToLocalOrTrustedExternalHostOnlyAsync(model.ApplicationName, model.ExternalUrl, model.Host);
                        }
                    }
                }

                await _auditLog.Log("FailedToCreateIdentity", $"Email '{model.Email}', ", HttpContext.Connection.RemoteIpAddress.ToString());

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

        public async Task<ExternalIdentity> Get(string authToken, string applicationKey)
        {
            var token = await _externalApplicationService.GetTokenAsync(authToken, applicationKey);
            var identity = await _identityService.GetIdentityAsync(token.IdentityId);
            var extensions = await GetIdentityAttributesAsExternsions(identity);

            var externalIdentity = new ExternalIdentity
            {
                GivenName = identity.GivenName,
                IdentityId = identity.IdentityId,
                Surname = identity.Surname,
                Email = identity.Email,
                PhoneNumber = identity.PhoneNumber,
                Issuer = RinsenIdentityConstants.RinsenIdentityProvider,
                Expiration = token.Expiration,
                CorrelationId = token.CorrelationId,
                Extensions = extensions
            };

            await _externalApplicationService.LogExportedExternalIdentity(externalIdentity, token.ExternalApplicationId);

            return externalIdentity;
        }

        private async Task<List<Extension>> GetIdentityAttributesAsExternsions(Identity identity)
        {
            var identityAttributes = await _identityAttributeStorage.GetIdentityAttributesAsync(identity.IdentityId);

            var extensions = new List<Extension>();

            if (identityAttributes.Any(attr => attr.Attribute == "Administrator"))
            {
                extensions.Add(new Extension { Type = RinsenIdentityConstants.Role, Value = RinsenIdentityConstants.Administrator });
            }

            return extensions;
        }
    }
}
