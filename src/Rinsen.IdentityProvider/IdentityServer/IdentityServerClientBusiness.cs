using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
using Rinsen.IdentityProvider.Core;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerClientBusiness
    {
        private readonly IdentityServerDbContext _identityServerDbContext;

        public IdentityServerClientBusiness(
            IdentityServerDbContext identityServerDbContext)
        {
            _identityServerDbContext = identityServerDbContext;
        }

        public Task<List<IdentityServerClient>> GetClients()
        {
            return _identityServerDbContext.IdentityServerClients
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.AllowedGrantTypes)
                .Include(m => m.AllowedScopes)
                .Include(m => m.Claims)
                .Include(m => m.ClientSecrets)
                .Include(m => m.IdentityProviderRestrictions)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.RedirectUris).ToListAsync();
        }

        public async Task<Client> GetClient(string clientId)
        {
            var identityServerClient = await GetIdentityServerClient(clientId);

            var client = new Client
            {
                AbsoluteRefreshTokenLifetime = identityServerClient.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = identityServerClient.AccessTokenLifetime,
                AccessTokenType = identityServerClient.AccessTokenType,
                AllowAccessTokensViaBrowser = identityServerClient.AllowAccessTokensViaBrowser,
                AllowOfflineAccess = identityServerClient.AllowOfflineAccess,
                AllowPlainTextPkce = identityServerClient.AllowPlainTextPkce,
                AllowRememberConsent = identityServerClient.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = identityServerClient.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = identityServerClient.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = identityServerClient.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = identityServerClient.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = identityServerClient.BackChannelLogoutUri,
                ClientClaimsPrefix = identityServerClient.ClientClaimsPrefix,
                ClientId = identityServerClient.ClientId,
                ClientName = identityServerClient.ClientName,
                ClientUri = identityServerClient.ClientUri,
                ConsentLifetime = identityServerClient.ConsentLifetime,
                Description = identityServerClient.Description,
                DeviceCodeLifetime = identityServerClient.DeviceCodeLifetime,
                Enabled = identityServerClient.Enabled,
                EnableLocalLogin = identityServerClient.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = identityServerClient.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = identityServerClient.FrontChannelLogoutUri,
                IdentityTokenLifetime = identityServerClient.IdentityTokenLifetime,
                IncludeJwtId = identityServerClient.IncludeJwtId,
                LogoUri = identityServerClient.LogoUri,
                PairWiseSubjectSalt = identityServerClient.PairWiseSubjectSalt,
                ProtocolType = identityServerClient.ProtocolType,
                RefreshTokenExpiration = identityServerClient.RefreshTokenExpiration,
                RefreshTokenUsage = identityServerClient.RefreshTokenUsage,
                RequireClientSecret = identityServerClient.RequireClientSecret,
                RequireConsent = identityServerClient.RequireConsent,
                RequirePkce = identityServerClient.RequirePkce,
                SlidingRefreshTokenLifetime = identityServerClient.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = identityServerClient.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = identityServerClient.UserCodeType,
                UserSsoLifetime = identityServerClient.UserSsoLifetime
            };

            client.AllowedCorsOrigins = identityServerClient.AllowedCorsOrigins.Select(aco => aco.Origin).ToArray();
            client.AllowedGrantTypes= identityServerClient.AllowedGrantTypes.Select(agt => agt.GrantType ).ToArray();
            client.AllowedScopes = identityServerClient.AllowedScopes.Select(s => s.Scope).ToArray();
            client.ClientSecrets = identityServerClient.ClientSecrets.Select(s => new Secret { Description = s.Description, Expiration = s.Expiration.HasValue ? s.Expiration.Value.DateTime : (DateTime?)null, Type = s.Type, Value = s.Value }).ToArray();
            client.IdentityProviderRestrictions = identityServerClient.IdentityProviderRestrictions.Select(s => s.Provider).ToArray();
            client.PostLogoutRedirectUris = identityServerClient.PostLogoutRedirectUris.Select(s => s.PostLogoutRedirectUri).ToArray();
            client.RedirectUris = identityServerClient.RedirectUris.Select(s => s.RedirectUri).ToArray();

            client.Claims = identityServerClient.Claims.Select(s => new Claim(s.Type, s.Value, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider)).ToList();
            client.Claims.Add(new Claim("name", identityServerClient.ClientName, ClaimValueTypes.String, RinsenIdentityConstants.RinsenIdentityProvider));

            return client;
        }

        public Task<List<IdentityServerClientType>> GetAllClientTypes()
        {
            var clientTypes = _identityServerDbContext.IdentityServerClientTypes.ToListAsync();

            return clientTypes;
        }

        public Task<IdentityServerClient> GetIdentityServerClient(string clientId)
        {
            var client = _identityServerDbContext.IdentityServerClients
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.AllowedGrantTypes)
                .Include(m => m.AllowedScopes)
                .Include(m => m.Claims)
                .Include(m => m.ClientSecrets)
                .Include(m => m.IdentityProviderRestrictions)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.RedirectUris).FirstOrDefaultAsync(m => m.ClientId == clientId);

            return client;
        }

        public async Task DeleteIdentityServerClient(string clientId)
        {
            var client = await _identityServerDbContext.IdentityServerClients
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.AllowedGrantTypes)
                .Include(m => m.AllowedScopes)
                .Include(m => m.Claims)
                .Include(m => m.ClientSecrets)
                .Include(m => m.IdentityProviderRestrictions)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.RedirectUris).FirstOrDefaultAsync(m => m.ClientId == clientId);

            _identityServerDbContext.IdentityServerClients.Remove(client);

            await _identityServerDbContext.SaveChangesAsync();
        }

        public Task<List<IdentityServerClient>> GetIdentityServerClients()
        {
            var clients = _identityServerDbContext.IdentityServerClients
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.AllowedGrantTypes)
                .Include(m => m.AllowedScopes)
                .Include(m => m.Claims)
                .Include(m => m.ClientSecrets)
                .Include(m => m.IdentityProviderRestrictions)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.RedirectUris).ToListAsync();

            return clients;
        }

        public Task<int> UpdateClient(IdentityServerClient identityServerClient)
        {
            foreach (var secret in identityServerClient.ClientSecrets.Where(s => s.Value != "****"))
            {
                secret.Value = secret.Value.Sha256();
            }

            return _identityServerDbContext.SaveAnnotatedGraphAsync(identityServerClient);
        }

        public async Task CreateNewClient(string clientId, string clientName, string description)
        {
            var exampleClient = new Client
            {
                ClientId = clientId,
                ClientName = clientName,
                Description = description
            };

            await SaveClient(exampleClient);
        }

        public async Task<string> CreateNewTypedClient(string clientName, string description, string typeName)
        {
            var exampleClient = new Client
            {
                ClientId = Guid.NewGuid().ToString(),
                ClientName = clientName,
                Description = description
            };
            var id = await GetClientTypeId(typeName);

            await SaveClient(exampleClient, id);

            return exampleClient.ClientId;
        }

        private async Task<int> GetClientTypeId(string typeName)
        {
            var identityServerClientType = await _identityServerDbContext.IdentityServerClientTypes.FirstOrDefaultAsync(m => m.Name == typeName);

            if (identityServerClientType == default)
            {
                identityServerClientType = new IdentityServerClientType
                {
                    Created = DateTimeOffset.Now,
                    Name = typeName,
                    Updated = DateTimeOffset.Now
                };

                await _identityServerDbContext.IdentityServerClientTypes.AddAsync(identityServerClientType);

                await _identityServerDbContext.SaveChangesAsync();
            }

            return identityServerClientType.Id;
        }

        private async Task SaveClient(Client clientToCreate, int? typeId = default)
        {
            var client = new IdentityServerClient
            {
                AbsoluteRefreshTokenLifetime = clientToCreate.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = clientToCreate.AccessTokenLifetime,
                AccessTokenType = clientToCreate.AccessTokenType,
                AllowAccessTokensViaBrowser = clientToCreate.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = new List<IdentityServerClientCorsOrigin>(),
                AllowedGrantTypes = new List<IdentityServerClientGrantType>(),
                AllowedScopes = new List<IdentityServerClientScope>(),
                AllowOfflineAccess = clientToCreate.AllowOfflineAccess,
                AllowPlainTextPkce = clientToCreate.AllowPlainTextPkce,
                AllowRememberConsent = clientToCreate.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = clientToCreate.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = clientToCreate.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = clientToCreate.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = clientToCreate.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = clientToCreate.BackChannelLogoutUri,
                Claims = new List<IdentityServerClientClaim>(),
                ClientClaimsPrefix = clientToCreate.ClientClaimsPrefix,
                ClientId = clientToCreate.ClientId,
                ClientName = clientToCreate.ClientName,
                ClientSecrets = new List<IdentityServerClientSecret>(),
                ClientTypeId = typeId,
                ClientUri = clientToCreate.ClientUri,
                ConsentLifetime = clientToCreate.ConsentLifetime,
                Description = clientToCreate.Description,
                DeviceCodeLifetime = clientToCreate.DeviceCodeLifetime,
                Enabled = clientToCreate.Enabled,
                EnableLocalLogin = clientToCreate.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = clientToCreate.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = clientToCreate.FrontChannelLogoutUri,
                IdentityProviderRestrictions = new List<IdentityServerClientIdpRestriction>(),
                IdentityTokenLifetime = clientToCreate.IdentityTokenLifetime,
                IncludeJwtId = clientToCreate.IncludeJwtId,
                LogoUri = clientToCreate.LogoUri,
                PairWiseSubjectSalt = clientToCreate.PairWiseSubjectSalt,
                PostLogoutRedirectUris = new List<IdentityServerClientPostLogoutRedirectUri>(),
                ProtocolType = clientToCreate.ProtocolType,
                RedirectUris = new List<IdentityServerClientRedirectUri>(),
                RefreshTokenExpiration = clientToCreate.RefreshTokenExpiration,
                RefreshTokenUsage = clientToCreate.RefreshTokenUsage,
                RequireClientSecret = clientToCreate.RequireClientSecret,
                RequireConsent = clientToCreate.RequireConsent,
                RequirePkce = clientToCreate.RequirePkce,
                SlidingRefreshTokenLifetime = clientToCreate.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = clientToCreate.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = clientToCreate.UserCodeType,
                UserSsoLifetime = clientToCreate.UserSsoLifetime,
            };

            client.AllowedCorsOrigins.AddRange(clientToCreate.AllowedCorsOrigins.Select(aco => new IdentityServerClientCorsOrigin { Origin = aco }));
            client.AllowedGrantTypes.AddRange(clientToCreate.AllowedGrantTypes.Select(agt => new IdentityServerClientGrantType { GrantType = agt }));
            client.AllowedScopes.AddRange(clientToCreate.AllowedScopes.Select(s => new IdentityServerClientScope { Scope = s }));
            client.Claims.AddRange(clientToCreate.Claims.Select(s => new IdentityServerClientClaim { Type = s.Type, Value = s.Value }));
            client.ClientSecrets.AddRange(clientToCreate.ClientSecrets.Select(s => new IdentityServerClientSecret { Description = s.Description, Expiration = s.Expiration, Type = s.Type, Value = s.Value }));
            client.IdentityProviderRestrictions.AddRange(clientToCreate.IdentityProviderRestrictions.Select(s => new IdentityServerClientIdpRestriction { Provider = s }));
            client.PostLogoutRedirectUris.AddRange(clientToCreate.PostLogoutRedirectUris.Select(s => new IdentityServerClientPostLogoutRedirectUri { PostLogoutRedirectUri = s }));

            _identityServerDbContext.IdentityServerClients.Add(client);

            await _identityServerDbContext.SaveChangesAsync();
        }
    }
}
