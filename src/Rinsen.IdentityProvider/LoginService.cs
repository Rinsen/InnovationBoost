﻿using IdentityModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider.Core;
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

        public LoginService(ILocalAccountService localAccountService,
            IIdentityService identityService,
            IIdentityAttributeStorage identityAttributeStorage,
            IHttpContextAccessor httpContextAccessor,
            RandomStringGenerator randomStringGenerator, 
            ILogger<LoginService> log)
        {
            _localAccountService = localAccountService;
            _identityService = identityService;
            _identityAttributeStorage = identityAttributeStorage;
            _httpContextAccessor = httpContextAccessor;
            _randomStringGenerator = randomStringGenerator;
            _log = log;
        }

        public async Task<LoginResult> LoginAsync(string email, string password, string host, bool rememberMe)
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
