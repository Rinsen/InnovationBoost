using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback;
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
            return _outbackDbContext.Clients
                .Include(m => m.AllowedCorsOrigins)
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
            return _outbackDbContext.Clients.Where(m => m.ClientId == clientId)
                .Include(m => m.AllowedCorsOrigins)
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
            var client = await _outbackDbContext.Clients.Where(m => m.Id == id)
                .Include(m => m.AllowedCorsOrigins)
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

            UpdateAllowedCorsOrigins(client, updatedClient);
            UpdateClientClaims(client, updatedClient);
            UpdateLoginRedirectUris(client, updatedClient);
            UpdatePostLogoutRedirectUris(client, updatedClient);
            UpdateScopes(client, updatedClient);
            UpdateSecrets(client, updatedClient);
            UpdateSupportedGrantTypes(client, updatedClient);

            await _outbackDbContext.SaveChangesAsync();
        }

        private void UpdateAllowedCorsOrigins(OutbackClient client, OutbackClient updatedClient)
        {
            var existingAllowedCorsOrigins = new List<OutbackClientAllowedCorsOrigin>();

            foreach (var allowedCorsOrigin in updatedClient.AllowedCorsOrigins)
            {
                var existingAllowedCorsOrigin = client.AllowedCorsOrigins.SingleOrDefault(m => m.Origin == allowedCorsOrigin.Origin);

                if (existingAllowedCorsOrigin == default)
                {
                    client.AllowedCorsOrigins.Add(new OutbackClientAllowedCorsOrigin
                    {
                        Origin = allowedCorsOrigin.Origin,
                        Description = allowedCorsOrigin.Description
                    });
                }
                else
                {
                    if (existingAllowedCorsOrigin.Deleted != null)
                    {
                        existingAllowedCorsOrigin.Deleted = null;
                    }

                    if (existingAllowedCorsOrigin.Description != allowedCorsOrigin.Description)
                    {
                        existingAllowedCorsOrigin.Description = allowedCorsOrigin.Description;
                    }

                    existingAllowedCorsOrigins.Add(existingAllowedCorsOrigin);
                }
            }

            var allowedCorsOriginsToDelete = client.AllowedCorsOrigins.Except(existingAllowedCorsOrigins).ToList();

            if (allowedCorsOriginsToDelete.Any())
            {
                _outbackDbContext.AllowedCorsOrigins.RemoveRange(allowedCorsOriginsToDelete);
            }
        }

        private void UpdateClientClaims(OutbackClient client, OutbackClient updatedClient)
        {
            var existingClientClaims = new List<OutbackClientClaim>();

            foreach (var clientClaim in updatedClient.ClientClaims)
            {
                var existingClientClaim = client.ClientClaims.SingleOrDefault(m => m.Type == clientClaim.Type);

                if (existingClientClaim == default)
                {
                    client.ClientClaims.Add(new OutbackClientClaim
                    {
                        Type = clientClaim.Type,
                        Description = clientClaim.Description,
                        Value = clientClaim.Value
                    });
                }
                else
                {
                    if (existingClientClaim.Deleted != null)
                    {
                        existingClientClaim.Deleted = null;
                    }

                    if (existingClientClaim.Description != clientClaim.Description ||
                    existingClientClaim.Value != clientClaim.Value)
                    {
                        existingClientClaim.Description = clientClaim.Description;
                        existingClientClaim.Value = clientClaim.Value;
                    }

                    existingClientClaims.Add(existingClientClaim);
                }
            }

            var existingClientClaimsToDelete = client.ClientClaims.Except(existingClientClaims).ToList();

            if (existingClientClaimsToDelete.Any())
            {
                _outbackDbContext.ClientClaims.RemoveRange(existingClientClaimsToDelete);
            }
        }

        private void UpdateLoginRedirectUris(OutbackClient client, OutbackClient updatedClient)
        {
            var existingLogonRedirectUris = new List<OutbackClientLoginRedirectUri>();

            foreach (var loginRedirectUri in updatedClient.LoginRedirectUris)
            {
                var existingLoginRedirectUri = client.LoginRedirectUris.SingleOrDefault(m => m.LoginRedirectUri == loginRedirectUri.LoginRedirectUri);

                if (existingLoginRedirectUri == default)
                {
                    client.LoginRedirectUris.Add(new OutbackClientLoginRedirectUri
                    {
                        Description = loginRedirectUri.Description,
                        LoginRedirectUri = loginRedirectUri.LoginRedirectUri
                    });
                }
                else
                {
                    if (existingLoginRedirectUri.Deleted != null)
                    {
                        existingLoginRedirectUri.Deleted = null;
                    }

                    if (existingLoginRedirectUri.Description != loginRedirectUri.Description ||
                        existingLoginRedirectUri.LoginRedirectUri != loginRedirectUri.LoginRedirectUri)
                    {
                        existingLoginRedirectUri.Description = loginRedirectUri.Description;
                        existingLoginRedirectUri.LoginRedirectUri = loginRedirectUri.LoginRedirectUri;
                    }
                }
            }

            var existingLogonRedirectUrisToDelete = client.LoginRedirectUris.Except(existingLogonRedirectUris).ToList();

            if (existingLogonRedirectUrisToDelete.Any())
            {
                _outbackDbContext.ClientLoginRedirectUris.RemoveRange(existingLogonRedirectUrisToDelete);
            }
        }

        private void UpdatePostLogoutRedirectUris(OutbackClient client, OutbackClient updatedClient)
        {
            var existingPostLogoutRedirectUris = new List<OutbackClientPostLogoutRedirectUri>();

            foreach (var postLogoutRedirectUri in updatedClient.PostLogoutRedirectUris)
            {
                var existingPostLogoutRedirectUri = client.PostLogoutRedirectUris.SingleOrDefault(m => m.PostLogoutRedirectUri == postLogoutRedirectUri.PostLogoutRedirectUri);

                if (existingPostLogoutRedirectUri == default)
                {
                    client.PostLogoutRedirectUris.Add(new OutbackClientPostLogoutRedirectUri
                    {
                        Description = postLogoutRedirectUri.Description,
                        PostLogoutRedirectUri = postLogoutRedirectUri.PostLogoutRedirectUri
                    });
                }
                else
                {
                    if (existingPostLogoutRedirectUri.Deleted != null)
                    {
                        existingPostLogoutRedirectUri.Deleted = null;
                    }

                    if (existingPostLogoutRedirectUri.Description != postLogoutRedirectUri.Description ||
                        existingPostLogoutRedirectUri.PostLogoutRedirectUri != postLogoutRedirectUri.PostLogoutRedirectUri)
                    {
                        existingPostLogoutRedirectUri.Description = postLogoutRedirectUri.Description;
                        existingPostLogoutRedirectUri.PostLogoutRedirectUri = postLogoutRedirectUri.PostLogoutRedirectUri;
                    }
                }
            }

            var existingPostLogoutRedirectUrisToDelete = client.PostLogoutRedirectUris.Except(existingPostLogoutRedirectUris).ToList();

            if (existingPostLogoutRedirectUrisToDelete.Any())
            {
                _outbackDbContext.ClientPostLogoutRedirectUris.RemoveRange(existingPostLogoutRedirectUrisToDelete);
            }
        }

        private void UpdateScopes(OutbackClient client, OutbackClient updatedClient)
        {
            var existingScopes = new List<OutbackClientScope>();

            foreach (var scope in updatedClient.Scopes)
            {
                var existingScope = client.Scopes.SingleOrDefault(m => m.ScopeId == scope.ScopeId);

                if (existingScope == default)
                {
                    client.Scopes.Add(new OutbackClientScope
                    {
                        ScopeId = scope.ScopeId
                    });
                }
                else
                {
                    if (existingScope.Deleted != null)
                    {
                        existingScope.Deleted = null;
                    }

                    existingScopes.Add(existingScope);
                }
            }

            var existingScopesToDelete = client.Scopes.Except(existingScopes).ToList();

            if (existingScopesToDelete.Any())
            {
                _outbackDbContext.ClientScopes.RemoveRange(existingScopesToDelete);
            }
        }

        private void UpdateSecrets(OutbackClient client, OutbackClient updatedClient)
        {
            var existingScopes = new List<OutbackClientSecret>();

            foreach (var secret in updatedClient.Secrets)
            {
                var existingSecret = client.Secrets.SingleOrDefault(m => m.Id == secret.Id);

                if (existingSecret == default)
                {
                    client.Secrets.Add(new OutbackClientSecret
                    {
                        Description = secret.Description,
                        Secret = HashHelper.GetSha256Hash(secret.Secret)
                    });
                }
                else
                {
                    if (existingSecret.Deleted != null)
                    {
                        existingSecret.Deleted = null;
                    }

                    if (existingSecret.Description != secret.Description)
                    {
                        existingSecret.Description = secret.Description;
                    }

                    existingScopes.Add(existingSecret);
                }
            }

            var existingSecretsToDelete = client.Secrets.Except(existingScopes).ToList();

            if (existingSecretsToDelete.Any())
            {
                _outbackDbContext.ClientSecrets.RemoveRange(existingSecretsToDelete);
            }
        }

        private void UpdateSupportedGrantTypes(OutbackClient client, OutbackClient updatedClient)
        {
            var existingGrantTypes = new List<OutbackClientSupportedGrantType>();

            foreach (var grantType in updatedClient.SupportedGrantTypes)
            {
                var existingGrantType = client.SupportedGrantTypes.SingleOrDefault(m => m.GrantType == grantType.GrantType);

                if (existingGrantType == default)
                {
                    client.SupportedGrantTypes.Add(new OutbackClientSupportedGrantType
                    {
                        GrantType = grantType.GrantType
                    });
                }
                else
                {
                    if (existingGrantType.Deleted != null)
                    {
                        existingGrantType.Deleted = null;
                    }

                    existingGrantTypes.Add(existingGrantType);
                }
            }

            var supportedGrantTypeToDelete = client.SupportedGrantTypes.Except(existingGrantTypes).ToList();

            if (supportedGrantTypeToDelete.Any())
            {
                _outbackDbContext.ClientSupportedGrantTypes.RemoveRange(supportedGrantTypeToDelete);
            }
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
    }
}
