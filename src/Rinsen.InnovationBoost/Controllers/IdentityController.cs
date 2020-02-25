using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.AuditLogging;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.InnovationBoost.Models;
using System.Security.Claims;
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
        private readonly IConfiguration _configuration;

        public IdentityController(ILoginService loginService,
            IIdentityService identityService,
            ILocalAccountService localAccountService,
            AuditLog auditLog,
            IIdentityServerInteractionService identityServerInteractionService,
            IConfiguration configuration)
        {
            _loginService = loginService;
            _identityService = identityService;
            _localAccountService = localAccountService;
            _auditLog = auditLog;
            _identityServerInteractionService = identityServerInteractionService;
            _configuration = configuration;
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
                    return await LoginSuccess(model.Email, model.ReturnUrl, result.Principal);
                }
                else if(result.TwoFactorRequired)
                {
                    model.RequestTwoFactor = true;
                    model.TwoFactorAppNotificationEnabled = result.TwoFactorAppNotificationEnabled;
                    model.TwoFactorEmailEnabled = result.TwoFactorEmailEnabled;
                    model.TwoFactorSmsEnabled = result.TwoFactorSmsEnabled;
                    model.TwoFactorTotpEnabled = result.TwoFactorTotpEnabled;

                    Response.Cookies.Append("AuthSessionId", result.TwoFactorAuthenticationSessionId);

                    return View("TwoFactor", model);
                }
                else
                {
                    await _auditLog.Log("InvalidLoginAttempt", $"Email '{model.Email}'", HttpContext.Connection.RemoteIpAddress.ToString());

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        private async Task<IActionResult> LoginSuccess(string email, string returnUrl, ClaimsPrincipal principal)
        {
            await _auditLog.Log("LoginSuccess", $"Email '{email}'", HttpContext.Connection.RemoteIpAddress.ToString());

            // Set loged in user to the one just created as this only will be provided at next request by the framework
            HttpContext.User = principal;

            if (_identityServerInteractionService.IsValidReturnUrl(returnUrl) || Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return Redirect("~/");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<IActionResult> TwoFactor(TwoFactorModel model)
        {
            if (ModelState.IsValid)
            {
                if(Request.Cookies.TryGetValue("AuthSessionId", out var authSessionId))
                {
                    if (model.TypeSelected == TwoFactorType.Totp)
                    {
                        if (string.IsNullOrEmpty(model.KeyCode))
                        {
                            await _loginService.StartTotpFlow(authSessionId);

                            return View(model);
                        }
                        var result = await _loginService.ConfirmTotpCode(authSessionId, model.KeyCode);

                        if (result.Succeeded)
                        {
                            return await LoginSuccess("", model.ReturnUrl, result.Principal);
                        }
                        else
                        {
                            await _auditLog.Log("InvalidLoginAttempt", $"Totp key code not valid", HttpContext.Connection.RemoteIpAddress.ToString());

                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        }
                    }
                }
                else
                {
                    await _auditLog.Log("InvalidLoginAttempt", $"Auth Session Id not found", HttpContext.Connection.RemoteIpAddress.ToString());
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
                if (model.InviteCode != _configuration["Rinsen:InvitationCode"])
                {
                    ModelState.AddModelError("InviteCode", "Invalid invite code.");

                    await _auditLog.Log("InvalidInvitationCode", $"Email '{model.Email}', '{model.InviteCode}'", HttpContext.Connection.RemoteIpAddress.ToString());

                    return View(model);
                }

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
