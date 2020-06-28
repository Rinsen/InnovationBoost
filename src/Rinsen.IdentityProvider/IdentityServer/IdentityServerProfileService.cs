using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerProfileService : IProfileService
    {
        private readonly ILogger<IdentityServerProfileService> _logger;
        private readonly IIdentityStorage _identityStorage;

        public IdentityServerProfileService(ILogger<IdentityServerProfileService> logger,
            IIdentityStorage identityStorage
            )
        {
            _logger = logger;
            _identityStorage = identityStorage;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var identity = await _identityStorage.GetAsync(context.Subject.GetClaimGuidValue(ClaimTypes.NameIdentifier));
            var authTime = context.Subject.GetClaimIntValue(JwtClaimTypes.AuthenticationTime);

            // https://github.com/IdentityServer/IdentityServer4/blob/master/src/IdentityServer4/src/Extensions/ProfileDataRequestContextExtensions.cs
            // https://github.com/IdentityServer/IdentityServer4/blob/master/src/IdentityServer4/src/Services/Default/DefaultProfileService.cs
            context.LogProfileRequest(_logger);

            // https://openid.net/specs/openid-connect-core-1_0.html#StandardClaims
            foreach (var requestedClaimType in context.RequestedClaimTypes)
            {
                switch (requestedClaimType)
                {
                    case JwtClaimTypes.Name:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Name, identity.GivenName + " " + identity.Surname, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.GivenName:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.GivenName, identity.GivenName, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.FamilyName:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.FamilyName, identity.Surname, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.Email:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Email, identity.Email, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.EmailVerified:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.EmailVerified, identity.EmailConfirmed.ToString(), ClaimValueTypes.Boolean, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.PhoneNumber:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.PhoneNumber, identity.PhoneNumber, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.PhoneNumberVerified:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.PhoneNumberVerified, identity.PhoneNumberConfirmed.ToString(), ClaimValueTypes.Boolean, RinsenIdentityConstants.RinsenIdentityProvider));
                        break;
                    case JwtClaimTypes.Expiration:
                        context.IssuedClaims.Add(new Claim(JwtClaimTypes.Expiration, DateTimeOffset.FromUnixTimeSeconds(authTime).AddSeconds(context.Client.IdentityTokenLifetime).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));
                        break;
                    default:
                        break;
                }
            }

            //context.AddRequestedClaims(context.Subject.Claims);
            context.LogIssuedClaims(_logger);
        }
        public Task IsActiveAsync(IsActiveContext context)
        {
            _logger.LogDebug("IsActive called from: {caller}", context.Caller);

            context.IsActive = true;
            return Task.CompletedTask;
        }
    }
}
