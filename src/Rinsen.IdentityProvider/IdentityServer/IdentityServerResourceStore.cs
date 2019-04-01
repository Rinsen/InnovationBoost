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
        public Task<ApiResource> FindApiResourceAsync(string name)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult(Enumerable.Empty<ApiResource>());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            return Task.FromResult<IEnumerable<IdentityResource>>(new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile()
            });
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            return Task.FromResult(new Resources
            {
                IdentityResources = new List<IdentityResource>
                {
                    new IdentityResources.OpenId(),
                    new IdentityResources.Profile()
                }
                //ApiResources = new List<ApiResource>
                //{
                    
                //}
            });
        }
    }
}
