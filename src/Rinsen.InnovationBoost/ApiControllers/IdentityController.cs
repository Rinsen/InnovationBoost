using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Rinsen.IdentityProvider;
using Rinsen.IdentityProvider.Contracts;
using Rinsen.IdentityProvider.Contracts.v1;
using Rinsen.IdentityProvider.ExternalApplications;

namespace Rinsen.InnovationBoost.ApiControllers
{
    [Route("api/v1/[controller]")]
    public class IdentityController : Controller
    {
        private readonly IIdentityAttributeStorage _identityAttributeStorage;
        private readonly IExternalApplicationService _externalApplicationService;
        private readonly IIdentityService _identityService;

        public IdentityController(IExternalApplicationService externalApplicationService,
            IIdentityService identityService,
            IIdentityAttributeStorage identityAttributeStorage)
        {
            _externalApplicationService = externalApplicationService;
            _identityService = identityService;
            _identityAttributeStorage = identityAttributeStorage;
        }

        [Route("[action]")]
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
