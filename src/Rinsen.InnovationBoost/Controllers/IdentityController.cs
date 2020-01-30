using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.AuditLogging;
using Rinsen.IdentityProvider.Contracts;
using Rinsen.IdentityProvider.Contracts.v1;
using Rinsen.IdentityProvider.Core;
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
        private readonly IIdentityService _identityService;
        private readonly ILocalAccountService _localAccountService;
        private readonly IIdentityAttributeStorage _identityAttributeStorage;
        private readonly AuditLog _auditLog;

        public IdentityController(ILoginService loginService,
            IIdentityService identityService,
            ILocalAccountService localAccountService,
            IIdentityAttributeStorage identityAttributeStorage,
            AuditLog auditLog)
        {
            _loginService = loginService;
            _identityService = identityService;
            _localAccountService = localAccountService;
            _identityAttributeStorage = identityAttributeStorage;
            _auditLog = auditLog;
        }

        [HttpGet] 
        [AllowAnonymous]
        public IActionResult Login()
        {
            var model = new LoginModel();

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



            return View();
        }
    }
}
