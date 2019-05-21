﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerApiResourceBusiness
    {
        private readonly IdentityServerDbContext _identityServerDbContext;

        public IdentityServerApiResourceBusiness(IdentityServerDbContext identityServerDbContext)
        {
            _identityServerDbContext = identityServerDbContext;
        }

        public Task<List<IdentityServerApiResource>> GetIdentityServerApiResources()
        {
            return _identityServerDbContext.IdentityServerApiResources
                .Include(m => m.Scopes)
                    .ThenInclude(scope => scope.Claims)
                .Include(m => m.ApiSecrets)
                .Include(m => m.Claims)
                .ToListAsync();
        }

        public Task<IdentityServerApiResource> GetIdentityServerApiResource(string name)
        {
            return _identityServerDbContext.IdentityServerApiResources
                .Include(m => m.Scopes)
                    .ThenInclude(scope => scope.Claims)
                .Include(m => m.ApiSecrets)
                .Include(m => m.Claims)
                .FirstOrDefaultAsync(ar => ar.Name == name);
        }

        public async Task<ApiResource> GetApiResourceAsync(string name)
        {
            var identityServerApiResource = await GetIdentityServerApiResource(name);

            var apiResource = MapApiResourceFromIdentityServerApiResource(identityServerApiResource);

            return apiResource;
        }

        private static ApiResource MapApiResourceFromIdentityServerApiResource(IdentityServerApiResource identityServerApiResource)
        {
            var apiResource = new ApiResource
            {
                Description = identityServerApiResource.Description,
                DisplayName = identityServerApiResource.DisplayName,
                Enabled = identityServerApiResource.Enabled,
                Name = identityServerApiResource.Name,
            };

            apiResource.ApiSecrets = identityServerApiResource.ApiSecrets.Select(s => new Secret { Description = s.Description, Expiration = s.Expiration.HasValue ? s.Expiration.Value.DateTime : (DateTime?)null, Type = s.Type, Value = s.Value }).ToArray();
            apiResource.Scopes = identityServerApiResource.Scopes.Select(s => new Scope
            {
                Description = s.Description,
                DisplayName = s.DisplayName,
                Emphasize = s.Emphasize,
                Name = s.Name,
                Required = s.Required,
                ShowInDiscoveryDocument = s.ShowInDiscoveryDocument,
                UserClaims = s.Claims.Select(c => c.Type).ToArray()
            }).ToArray();

            return apiResource;
        }

        public async Task<List<ApiResource>> GetApiResourcesAsync()
        {
            var identityServerApiResources = await GetIdentityServerApiResources();

            return identityServerApiResources.Select(isar => MapApiResourceFromIdentityServerApiResource(isar)).ToList();
        }

        public async Task CreateNewApiResource(string name, string displayName, string description)
        {
            var identityServerApiResource = new IdentityServerApiResource
            {
                Description = description,
                Enabled = true,
                DisplayName = displayName,
                Name = name,
            };

            await _identityServerDbContext.IdentityServerApiResources.AddAsync(identityServerApiResource);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public Task UpdateApiResource(IdentityServerApiResource identityServerApiResource)
        {
            return _identityServerDbContext.SaveAnnotatedGraphAsync(identityServerApiResource);
        }
    }
}