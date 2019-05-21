using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerResourceStore : IResourceStore
    {
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;

        public IdentityServerResourceStore(IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness,
            IdentityServerApiResourceBusiness identityServerApiResourceBustiness)
        {
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
            _identityServerApiResourceBusiness = identityServerApiResourceBustiness;
        }

        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            return _identityServerApiResourceBusiness.GetApiResourceAsync(name);
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            throw new NotImplementedException();
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
