using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.AuditLogging;
using Rinsen.IdentityProvider.LocalAccounts;
using Rinsen.InnovationBoost.Models;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using UAParser;

namespace Rinsen.InnovationBoost.Controllers
{
    public class IdentityController : Controller
    {
        private readonly ILoginService _loginService;
        private readonly IIdentityService _identityService;
        private readonly ILocalAccountService _localAccountService;
        private readonly AuditLog _auditLog;
        //private readonly IIdentityServerInteractionService _identityServerInteractionService;
        private readonly IConfiguration _configuration;
        private readonly IIdentityAccessor _identityAccessor;
        private readonly ISessionStorage _sessionStorage;

        public IdentityController(ILoginService loginService,
            IIdentityService identityService,
            ILocalAccountService localAccountService,
            AuditLog auditLog,
            //IIdentityServerInteractionService identityServerInteractionService,
            IConfiguration configuration,
            IIdentityAccessor identityAccessor,
            ISessionStorage sessionStorage)
        {
            _loginService = loginService;
            _identityService = identityService;
            _localAccountService = localAccountService;
            _auditLog = auditLog;
            //_identityServerInteractionService = identityServerInteractionService;
            _configuration = configuration;
            _identityAccessor = identityAccessor;
            _sessionStorage = sessionStorage;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var parser = Parser.GetDefault();
            var sessions = await _sessionStorage.GetAsync(_identityAccessor.IdentityId);
            var sessionId = _identityAccessor.ClaimsPrincipal.GetClaimStringValue("sid");

            return View(new IdentityOverview
            {
                Sessions = sessions.Select(m =>
                {
                    var client = parser.Parse(m.UserAgent);
                    
                    return new SessionModel
                    {
                        Id = m.Id,
                        ClientDescrition = client.ToString(),
                        IpAddress = m.IpAddress,
                        Expires = m.Expires,
                        Created = m.Created,
                        CurrentSession = m.SessionId == sessionId
                    };
                }).ToList()
            });
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
                    return await LoginSuccess(result.LoginId, model.ReturnUrl, result.Principal);
                }
                else if(result.TwoFactorRequired)
                {
                    var twoFactorModel = new TwoFactorModel
                    {
                        TypeSelected = TwoFactorType.Totp,
                        TwoFactorAppNotificationEnabled = result.TwoFactorAppNotificationEnabled,
                        ReturnUrl = model.ReturnUrl,
                        RememberMe = model.RememberMe,
                        TwoFactorEmailEnabled = result.TwoFactorEmailEnabled,
                        TwoFactorSmsEnabled = result.TwoFactorSmsEnabled,
                        TwoFactorTotpEnabled = result.TwoFactorTotpEnabled,
                    };
                    
                    Response.Cookies.Append("AuthSessionId", result.TwoFactorAuthenticationSessionId);

                    return View("TwoFactor", twoFactorModel);
                }
                else
                {
                    await CreateAuditLogEvent("InvalidLoginAttempt", $"Email '{model.Email}'");

                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                }
            }

            return View(model);
        }

        private async Task<IActionResult> LoginSuccess(string loginId, string returnUrl, ClaimsPrincipal principal)
        {
            await CreateAuditLogEvent("LoginSuccess", $"LoginId '{loginId}'");

            // Set loged in user to the one just created as this only will be provided at next request by the framework
            HttpContext.User = principal;

            if (/*_identityServerInteractionService.IsValidReturnUrl(returnUrl) || */Url.IsLocalUrl(returnUrl))
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

                        Response.Cookies.Delete("AuthSessionId");
                        LoginResult result;
                        try
                        {
                            result = await _loginService.ConfirmTotpCode(authSessionId, model.KeyCode, Request.Host.Value, model.RememberMe);
                        }
                        catch (TotpCodeAlreadyUsedException)
                        {
                            await CreateAuditLogEvent("TotpCodeUsed", $"Code {model.KeyCode} is already used");
                            throw;
                        }

                        if (result.Succeeded)
                        {
                            return await LoginSuccess(result.LoginId, model.ReturnUrl, result.Principal);
                        }
                        else
                        {
                            await CreateAuditLogEvent("InvalidLoginAttempt", $"Totp key code not valid");

                            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        }
                    }
                }
                else
                {
                    await CreateAuditLogEvent("InvalidLoginAttempt", $"Auth Session Id not found");
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

                    await CreateAuditLogEvent("InvalidInvitationCode", $"Email '{model.Email}', '{model.InviteCode}'");

                    return View(model);
                }

                var createIdentityResult = await _identityService.CreateAsync(model.GivenName, model.Surname, model.Email, model.PhoneNumber);

                if (createIdentityResult.Succeeded)
                {
                    await CreateAuditLogEvent("IdentityCreated", $"Email '{model.Email}'");

                    var createLocalAccountResult = await _localAccountService.CreateAsync(createIdentityResult.Identity.IdentityId, model.Email, model.Password);

                    if (createLocalAccountResult.Succeeded)
                    {
                        var loginResult = await _loginService.LoginAsync(model.Email, model.Password, Request.Host.Value, false);

                        if (loginResult.Succeeded)
                        {
                            return View("UserCreated");
                        }

                        return View("UserCreated");
                    }
                }

                await CreateAuditLogEvent("FailedToCreateIdentity", $"Email '{model.Email}'");

                ModelState.AddModelError(string.Empty, "Create user invalid.");
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> EnableTotp()
        {
            var totpSecret = await _localAccountService.EnableTotp();

            return Ok(totpSecret);
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

        private Task CreateAuditLogEvent(string eventType, string details)
        {
            return _auditLog.Log(eventType, details, HttpContext.Connection.RemoteIpAddress.ToString());
        }
    }
}
