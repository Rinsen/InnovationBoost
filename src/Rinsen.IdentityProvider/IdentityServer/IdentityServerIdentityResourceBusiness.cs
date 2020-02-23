using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerIdentityResourceBusiness
    {
        private readonly IdentityServerDbContext _identityServerDbContext;

        public IdentityServerIdentityResourceBusiness(IdentityServerDbContext identityServerDbContext)
        {
            _identityServerDbContext = identityServerDbContext;
        }

        public Task<List<IdentityServerIdentityResource>> GetIdentityServerIdentityResourcesAsync()
        {
            return _identityServerDbContext.IdentityServerIdentityResources
                .Include(m => m.Claims)
                .Include(m => m.Properties)
                .ToListAsync();
        }

        public Task<IdentityServerIdentityResource> GetIdentityServerIdentityResourceAsync(string name)
        {
            return _identityServerDbContext.IdentityServerIdentityResources
                .Include(m => m.Claims)
                .Include(m => m.Properties)
                .FirstOrDefaultAsync(m => m.Name == name);
        }

        public async Task DeleteIdentityServerIdentityResourceAsync(string name)
        {
            var identityResource = await _identityServerDbContext.IdentityServerIdentityResources
                .Include(m => m.Claims)
                .Include(m => m.Properties)
                .FirstOrDefaultAsync(m => m.Name == name);

            _identityServerDbContext.IdentityServerIdentityResources.Remove(identityResource);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public async Task<List<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
        {
            var identityResources = await _identityServerDbContext.IdentityServerIdentityResources
                .Include(m => m.Claims)
                .Include(m => m.Properties)
                .Where(m => scopeNames.Contains(m.Name)).ToListAsync();

            return identityResources.Select(ir => MapIdentityResourcesFromIdentityServerIdentityResources(ir)).ToList(); ;
        }

        public async Task<List<IdentityResource>> GetIdentityResourcesAsync()
        {
            var identityServerIdentityResources = await GetIdentityServerIdentityResourcesAsync();

            return identityServerIdentityResources.Select(ir => MapIdentityResourcesFromIdentityServerIdentityResources(ir)).ToList();
        }

        private IdentityResource MapIdentityResourcesFromIdentityServerIdentityResources(IdentityServerIdentityResource identityServerIdentityResource)
        {
            return new IdentityResource
            {
                Description = identityServerIdentityResource.Description,
                DisplayName = identityServerIdentityResource.DisplayName,
                Emphasize = identityServerIdentityResource.Emphasize,
                Enabled = identityServerIdentityResource.Enabled,
                Name = identityServerIdentityResource.Name,
                Required = identityServerIdentityResource.Required,
                ShowInDiscoveryDocument = identityServerIdentityResource.ShowInDiscoveryDocument,
                UserClaims = identityServerIdentityResource.Claims.Select(c => c.Type).ToArray()
            };
        }

        public async Task CreateNewIdentityResourceAsync(string name, string displayName, string description)
        {
            await _identityServerDbContext.IdentityServerIdentityResources.AddAsync(new IdentityServerIdentityResource
            {
                Description = description,
                DisplayName = displayName,
                Name = name,
                Enabled = true,
                ShowInDiscoveryDocument = false,
                Emphasize = false,
                Required = false
            });

            await _identityServerDbContext.SaveChangesAsync();
        }

        public Task<int> UpdateIdentityResourceAsync(IdentityServerIdentityResource identityServerIdentityResource)
        {
            return _identityServerDbContext.SaveAnnotatedGraphAsync(identityServerIdentityResource);
        }
    }
}
