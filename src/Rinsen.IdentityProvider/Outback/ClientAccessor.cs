using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Outback.Entities;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback
{
    public class ClientAccessor : IClientAccessor
    {
        private readonly OutbackDbContext _outbackDbContext;

        public ClientAccessor(OutbackDbContext outbackDbContext)
        {
            _outbackDbContext = outbackDbContext;
        }

        public async Task<Client> GetClient(string clientId)
        {
            var outbackClient = await _outbackDbContext.Clients
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.ClientClaims)
                .Include(m => m.LoginRedirectUris)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.Secrets)
                .Include(m => m.SupportedGrantTypes)
                .Include(m => m.Scopes).ThenInclude(m => m.Scope).SingleOrDefaultAsync(x => x.ClientId == clientId);

            if (outbackClient == default)
            {
                throw new Exception($"Client {clientId} not found");
            }

            return new Client
            {
                AccessTokenLifetime = outbackClient.AccessTokenLifetime,
                AllowedCorsOrigins = outbackClient.AllowedCorsOrigins.Select(m => m.Origin).ToList(),
                AuthorityCodeLifetime = outbackClient.AuthorityCodeLifetime,
                ClientClaims = outbackClient.ClientClaims.Select(m => new ClientClaim { Type = m.Type, Value = m.Value }).ToList(),
                ClientId = outbackClient.ClientId,
                ClientType = outbackClient.ClientType,
                ConsentRequired = outbackClient.ConsentRequired,
                IdentityTokenLifetime = outbackClient.IdentityTokenLifetime,
                IssueRefreshToken = outbackClient.IssueRefreshToken,
                LoginRedirectUris = outbackClient.LoginRedirectUris.Select(m => m.LoginRedirectUri).ToList(),
                PostLogoutRedirectUris = outbackClient.PostLogoutRedirectUris.Select(m => m.PostLogoutRedirectUri).ToList(),
                RefreshTokenLifetime = outbackClient.RefreshTokenLifetime,
                SaveConsent = outbackClient.SaveConsent,
                SavedConsentLifetime = outbackClient.SavedConsentLifetime,
                Scopes = outbackClient.Scopes.Select(m => m.Scope.ScopeName).ToList(),
                Secrets = outbackClient.Secrets.Select(m => m.Secret).ToList(),
                SupportedGrantTypes = outbackClient.SupportedGrantTypes.Select(m => m.GrantType).ToList()
            };
        }
    }
}
