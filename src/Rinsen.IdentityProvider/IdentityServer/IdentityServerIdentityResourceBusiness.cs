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
            await _identityServerDbContext.IdentityServerApiResources.AddAsync(new IdentityServerApiResource
            {
                Description = description,
                DisplayName = displayName,
                Name = name,
                Enabled = true
            });

            await _identityServerDbContext.SaveChangesAsync();
        }

        public Task UpdateIdentityResourceAsync(IdentityServerIdentityResource identityServerIdentityResource)
        {
            return _identityServerDbContext.SaveAnnotatedGraphAsync(identityServerIdentityResource);
        }
    }
}
