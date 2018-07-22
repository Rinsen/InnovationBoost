using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.LocalAccounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Rinsen.InnovationBoost
{
    public class IdentityWebLoginService : LoginService
    {
        private readonly IIdentityAttributeStorage _identityAttributeStorage;

        public IdentityWebLoginService(ILocalAccountService localAccountService,
            IIdentityService identityService,
            IHttpContextAccessor httpContextAccessor,
            IIdentityAttributeStorage identityAttributeStorage,
            ILogger<LoginService> logger)
            : base(localAccountService, identityService, httpContextAccessor, logger)
        {
            _identityAttributeStorage = identityAttributeStorage;
        }

        protected override async Task AddApplicationSpecificClaimsAsync(List<Claim> claims)
        {
            var identityAttributes = await _identityAttributeStorage.GetIdentityAttributesAsync(Guid.Parse(claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value));

            if (identityAttributes.Any(m => m.Attribute == "Administrator"))
            {
                claims.Add(new Claim("http://rinsen.se/Administrator", "True"));
            }
        }
    }
}
