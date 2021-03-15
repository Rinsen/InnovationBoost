using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback
{
    public class ClientService
    {
        private readonly OutbackDbContext _outbackDbContext;

        public ClientService(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }
        
        public Task<List<OutbackClient>> GetAll()
        {
            return _outbackDbContext.Clients.Include(m => m.AllowedCorsOrigins)
                .Include(m => m.ClientClaims)
                .Include(m => m.ClientFamily)
                .Include(m => m.LoginRedirectUris)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.Scopes).ThenInclude(m => m.Scope)
                .Include(m => m.Secrets)
                .Include(m => m.SupportedGrantTypes).ToListAsync();
        }

        public Task<OutbackClient> GetClient(string clientId)
        {
            return _outbackDbContext.Clients.Where(m => m.ClientId == clientId).Include(m => m.AllowedCorsOrigins)
                .Include(m => m.ClientClaims)
                .Include(m => m.ClientFamily)
                .Include(m => m.LoginRedirectUris)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.Scopes)
                .Include(m => m.Secrets)
                .Include(m => m.SupportedGrantTypes).SingleOrDefaultAsync();
        }

        public async Task DeleteClient(string clientId)
        {
            var client = await _outbackDbContext.Clients.SingleOrDefaultAsync(m => m.ClientId == clientId);

            if (client == default)
            {
                throw new Exception($"Client with id {clientId} not found");
            }

            _outbackDbContext.Clients.Remove(client);

            await _outbackDbContext.SaveChangesAsync();
        }

        public Task<List<OutbackClientFamily>> GetAllClientFamilies()
        {
            return _outbackDbContext.ClientFamilies.ToListAsync();
        }

        public async Task CreateNewClient(string clientId, string clientName, string description, int familyId, ClientType clientType)
        {
            var outbackClient = new OutbackClient
            {
                ClientId = clientId,
                ClientType = clientType,
                Description = description,
                Name = clientName,
                ClientFamilyId = familyId
            };

            await _outbackDbContext.AddAsync(outbackClient);

            await _outbackDbContext.SaveChangesAsync();
        }

        public async Task UpdateClient(int id, OutbackClient updatedClient)
        {
            var client = await _outbackDbContext.Clients.Where(m => m.Id == id).Include(m => m.AllowedCorsOrigins)
                .Include(m => m.ClientClaims)
                .Include(m => m.ClientFamily)
                .Include(m => m.LoginRedirectUris)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.Scopes)
                .Include(m => m.Secrets)
                .Include(m => m.SupportedGrantTypes).SingleOrDefaultAsync();

            if (ClientPropertiesEquals(client, updatedClient))
            {
                UpdateClientProperties(client, updatedClient);
            }

            await _outbackDbContext.SaveChangesAsync();
        }

        private static void UpdateClientProperties(OutbackClient client, OutbackClient updatedClient)
        {
            client.AccessTokenLifetime = updatedClient.AccessTokenLifetime;
            client.AuthorityCodeLifetime = updatedClient.AccessTokenLifetime;
            client.ClientFamilyId = updatedClient.ClientFamilyId;
            client.ClientType = updatedClient.ClientType;
            client.ConsentRequired = updatedClient.ConsentRequired;
            client.Description = updatedClient.Description;
            client.IdentityTokenLifetime = updatedClient.IdentityTokenLifetime;
            client.IssueRefreshToken = updatedClient.IssueRefreshToken;
            client.Name = updatedClient.Name;
            client.SaveConsent = updatedClient.SaveConsent;
            client.SavedConsentLifetime = updatedClient.SavedConsentLifetime;
        }

        public async Task<OutbackClientFamily> CreateNewFamily(string name, string description)
        {
            var outbackClientFamily = new OutbackClientFamily
            {
                Description = description,
                Name = name,
            };

            await _outbackDbContext.AddAsync(outbackClientFamily);

            await _outbackDbContext.SaveChangesAsync();

            return outbackClientFamily;
        }

        private static bool ClientPropertiesEquals(OutbackClient client, OutbackClient updatedClient)
        {
            return client.AccessTokenLifetime == updatedClient.AccessTokenLifetime
                && client.AuthorityCodeLifetime == updatedClient.AccessTokenLifetime
                && client.ClientFamilyId == updatedClient.ClientFamilyId
                && client.ClientType == updatedClient.ClientType
                && client.ConsentRequired == updatedClient.ConsentRequired
                && client.Description == updatedClient.Description
                && client.IdentityTokenLifetime == updatedClient.IdentityTokenLifetime
                && client.IssueRefreshToken == updatedClient.IssueRefreshToken
                && client.Name == updatedClient.Name
                && client.SaveConsent == updatedClient.SaveConsent
                && client.SavedConsentLifetime == updatedClient.SavedConsentLifetime;
        }
    }
}
