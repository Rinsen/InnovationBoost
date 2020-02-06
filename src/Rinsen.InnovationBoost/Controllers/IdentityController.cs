using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.AuditLogging;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.InnovationBoost.Models;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IIdentityService _identityService;
        private readonly ILocalAccountService _localAccountService;
        private readonly AuditLog _auditLog;
        private readonly IIdentityServerInteractionService _identityServerInteractionService;

        public IdentityController(ILoginService loginService,
            IIdentityService identityService,
            ILocalAccountService localAccountService,
            AuditLog auditLog,
            IIdentityServerInteractionService identityServerInteractionService)
        {
            _loginService = loginService;
            _identityService = identityService;
            _localAccountService = localAccountService;
            _auditLog = auditLog;
            _identityServerInteractionService = identityServerInteractionService;
        }

        [HttpGet] 
        [AllowAnonymous]
        public IActionResult Login(string returnUrl)
        {
            var model = new LoginModel
            {
                ReturnUrl = returnUrl
            };

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

                    if (_identityServerInteractionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }

                    return Redirect("~/");
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
            return SignOut(new Microsoft.AspNetCore.Authentication.AuthenticationProperties { RedirectUri = "/Identity/LoggedOut" }, CookieAuthenticationDefaults.AuthenticationScheme);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult LoggedOut()
        {
            return View();
        }
    }
}
