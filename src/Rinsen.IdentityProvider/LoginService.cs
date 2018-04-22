using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.LocalAccounts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.IdentityProvider
{
    public class LoginService : ILoginService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IIdentityService _identityService;
        private readonly ILogger<LoginService> _log;
        private readonly ILocalAccountService _localAccountService;

        public LoginService(ILocalAccountService localAccountService,
            IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor, 
            ILogger<LoginService> log)
        {
            _localAccountService = localAccountService;
            _httpContextAccessor = httpContextAccessor;
            _identityService = identityService;
            _log = log;
        }

        public async Task<LoginResult> LoginAsync(string email, string password, bool rememberMe)
        {
            Guid? identityId;
            try
            {
                identityId = await _localAccountService.GetIdentityIdAsync(email, password);
            }
            catch (UnauthorizedAccessException e)
            {
                _log.LogWarning(e, $"Login failed for email {email}", email);

                return LoginResult.Failure();
            }
            
            if (identityId == null)
            {
                return LoginResult.Failure();
            }

            var identity = await _identityService.GetIdentityAsync((Guid)identityId);

            var claims = await GetClaimsForIdentityAsync(identity, rememberMe);

            var authenticationProperties = new AuthenticationProperties()
            {
                AllowRefresh = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMonths(1),
                IsPersistent = rememberMe,
                IssuedUtc = DateTimeOffset.UtcNow,
            };

            var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, "RinsenPassword"));

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authenticationProperties);

            return LoginResult.Success(principal);
        }

        private async Task<List<Claim>> GetClaimsForIdentityAsync(Identity identity, bool rememberMe)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, identity.GivenName + " " + identity.Surname, ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.NameIdentifier, identity.IdentityId.ToString(), ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.Email, identity.Email, ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.GivenName, identity.GivenName, ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.Surname, identity.Surname, ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.Expiration, rememberMe.ToString(), ClaimValueTypes.String, "RinsenIdentityProvider"),
                new Claim(ClaimTypes.SerialNumber, Guid.NewGuid().ToString(), ClaimValueTypes.String, "RinsenIdentityProvider"),
            };

            await AddApplicationSpecificClaimsAsync(claims);

            return claims;
        }

        protected virtual Task AddApplicationSpecificClaimsAsync(List<Claim> claims)
        {
            return Task.CompletedTask;
        }

        public Task LogoutAsync()
        {
            throw new NotImplementedException();
        }
    }
}
