using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.Extensions.Logging;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerResourceStore : IResourceStore
    {
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;
        private readonly ILogger<IdentityServerResourceStore> _logger;

        public IdentityServerResourceStore(IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness,
            IdentityServerApiResourceBusiness identityServerApiResourceBustiness,
            ILogger<IdentityServerResourceStore> logger)
        {
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
            _identityServerApiResourceBusiness = identityServerApiResourceBustiness;
            _logger = logger;
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return _identityServerApiResourceBusiness.GetApiResourceAsync(name);
        }

        public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await _identityServerApiResourceBusiness.FindApiResourcesByScopeAsync(scopeNames);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return await _identityServerIdentityResourceBusiness.FindIdentityResourcesByScopeAsync(scopeNames);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiResources = await _identityServerApiResourceBusiness.GetApiResourcesAsync();
            var identityResources = await _identityServerIdentityResourceBusiness.GetIdentityResourcesAsync();

            return new Resources
            {
                IdentityResources = identityResources,
                ApiResources = apiResources
            };
        }
    }
}
