using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.LocalAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly RandomStringGenerator _randomStringGenerator;
        private readonly IIdentityService _identityService;
        private readonly ILogger<LoginService> _log;
        private readonly ILocalAccountService _localAccountService;
        private readonly IIdentityAttributeStorage _identityAttributeStorage;
        private readonly ITwoFactorAuthenticationSessionStorage _twoFactorAuthenticationSessionStorage;

        public LoginService(ILocalAccountService localAccountService,
            IIdentityService identityService,
            IIdentityAttributeStorage identityAttributeStorage,
            ITwoFactorAuthenticationSessionStorage twoFactorAuthenticationSessionStorage,
            IHttpContextAccessor httpContextAccessor,
            RandomStringGenerator randomStringGenerator, 
            ILogger<LoginService> log)
        {
            _localAccountService = localAccountService;
            _identityService = identityService;
            _identityAttributeStorage = identityAttributeStorage;
            _twoFactorAuthenticationSessionStorage = twoFactorAuthenticationSessionStorage;
            _httpContextAccessor = httpContextAccessor;
            _randomStringGenerator = randomStringGenerator;
            _log = log;
        }

        public async Task<LoginResult> LoginAsync(string email, string password, string host, bool rememberMe)
        {
            LocalAccount localAccount;
            try
            {
                localAccount = await _localAccountService.GetLocalAccountAsync(email, password);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.LogWarning(e, $"Login failed for email {email}", email);

                return LoginResult.Failure();
            }

            if (localAccount == null)
            {
                return LoginResult.Failure();
            }

            if (localAccount.TwoFactorAppNotificationEnabled || localAccount.TwoFactorEmailEnabled
                || localAccount.TwoFactorSmsEnabled || localAccount.TwoFactorTotpEnabled)
            {
                string keyCode = string.Empty;
                if (localAccount.TwoFactorEmailEnabled || localAccount.TwoFactorSmsEnabled)
                {
                    keyCode = _randomStringGenerator.GetRandomString(10);
                }

                var sessionId = _randomStringGenerator.GetRandomString(50);
                var twoFactorAuthenticationSession = new TwoFactorAuthenticationSession
                {
                    Created = DateTimeOffset.Now,
                    IdentityId = localAccount.IdentityId,
                    KeyCode = keyCode,
                    SessionId = sessionId,
                    Type = TwoFactorType.NotSelected
                };

                await _twoFactorAuthenticationSessionStorage.Create(twoFactorAuthenticationSession);

                return LoginResult.RequireTwoFactor(twoFactorAuthenticationSession.SessionId, localAccount.TwoFactorEmailEnabled, localAccount.TwoFactorSmsEnabled, localAccount.TwoFactorTotpEnabled, localAccount.TwoFactorAppNotificationEnabled);
            }

            return await PrivateLogin(host, rememberMe, localAccount);
        }

        public async Task StartTotpFlow(string authSessionId)
        {
            var twoFactorAuthenticationSession = await _twoFactorAuthenticationSessionStorage.Get(authSessionId);

            if (twoFactorAuthenticationSession is object)
            {
                twoFactorAuthenticationSession.Type = TwoFactorType.Totp;

                await _twoFactorAuthenticationSessionStorage.Update(twoFactorAuthenticationSession);
            }
            else
            {
                throw new Exception("Two factor auth session not found");
            }
        }

        public Task<LoginResult> ConfirmTotpCode(string authSessionId, string keyCode)
        {
            throw new NotImplementedException();
        }

        private async Task<LoginResult> PrivateLogin(string host, bool rememberMe, LocalAccount localAccount)
        {
            var identity = await _identityService.GetIdentityAsync(localAccount.IdentityId);

            var expiration = DateTimeOffset.UtcNow.AddMonths(1);
            var timeToExpiration = expiration.Subtract(DateTimeOffset.Now);
            var claims = await GetClaimsForIdentityAsync(identity, host, rememberMe, timeToExpiration);

            var authenticationProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                ExpiresUtc = expiration,
                IsPersistent = rememberMe,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "RinsenPassword"));

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

            return LoginResult.Success(principal);
        }

        private async Task<List<Claim>> GetClaimsForIdentityAsync(Identity identity, string host, bool rememberMe, TimeSpan timeToExpiration)
        {
            var sessionId = _randomStringGenerator.GetRandomString(32);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identity.GivenName + " " + identity.Surname, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.NameIdentifier, identity.IdentityId.ToString(), ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.Email, identity.Email, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.GivenName, identity.GivenName, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.Surname, identity.Surname, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.IsPersistent, rememberMe.ToString(), ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.Expiration, timeToExpiration.ToString(), ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(ClaimTypes.SerialNumber, Guid.NewGuid().ToString(), ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(JwtClaimTypes.Issuer, host, ClaimValueTypes.String,  RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(JwtClaimTypes.Subject, identity.IdentityId.ToString(), ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider),
                new Claim(JwtClaimTypes.SessionId, sessionId, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider)
            };

            var identityAttributes = await _identityAttributeStorage.GetIdentityAttributesAsync(Guid.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

            if (identityAttributes.Any(m => m.Attribute == "Administrator"))
            {
                claims.Add(new Claim(RinsenClaimTypes.Administrator, "True", ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
            }

            return claims;
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
