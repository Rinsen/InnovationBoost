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
    }
}
