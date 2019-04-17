using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.EntityFrameworkCore;
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
            throw new NotImplementedException();

        }

        public Task<IdentityServerClient> GetClient(string clientId)
        {
            var client = _identityServerDbContext.IdentityServerClients.Where(m => m.ClientId == clientId)
                .Include(m => m.AllowedCorsOrigins)
                .Include(m => m.AllowedGrantTypes)
                .Include(m => m.AllowedScopes)
                .Include(m => m.Claims)
                .Include(m => m.ClientSecrets)
                .Include(m => m.IdentityProviderRestrictions)
                .Include(m => m.PostLogoutRedirectUris)
                .Include(m => m.RedirectUris).FirstOrDefaultAsync();

            return client;
        }

        public async Task CreateTestData()
        {
            var exampleClient = new Client
            {
                ClientId = "mvc",
                ClientName = "MVC Client",
                ClientSecrets = { new Secret("secret".Sha256()) },
                RequireConsent = false,
                AllowedGrantTypes = GrantTypes.Implicit,

                // where to redirect to after login
                RedirectUris = { "https://localhost:44315/signin-oidc" },

                // where to redirect to after logout
                PostLogoutRedirectUris = { "https://localhost:44315/signout-callback-oidc" },

                AllowedScopes = new List<string>
                        {
                            IdentityServerConstants.StandardScopes.OpenId,
                            IdentityServerConstants.StandardScopes.Profile
                        }
            };

            var client = new IdentityServerClient
            {
                AbsoluteRefreshTokenLifetime = exampleClient.AbsoluteRefreshTokenLifetime,
                AccessTokenLifetime = exampleClient.AccessTokenLifetime,
                AccessTokenType = exampleClient.AccessTokenType,
                AllowAccessTokensViaBrowser = exampleClient.AllowAccessTokensViaBrowser,
                AllowedCorsOrigins = new List<IdentityServerClientCorsOrigin>(),
                AllowedGrantTypes = new List<IdentityServerClientGrantType>(),
                AllowedScopes = new List<IdentityServerClientScope>(),
                AllowOfflineAccess = exampleClient.AllowOfflineAccess,
                AllowPlainTextPkce = exampleClient.AllowPlainTextPkce,
                AllowRememberConsent = exampleClient.AllowRememberConsent,
                AlwaysIncludeUserClaimsInIdToken = exampleClient.AlwaysIncludeUserClaimsInIdToken,
                AlwaysSendClientClaims = exampleClient.AlwaysSendClientClaims,
                AuthorizationCodeLifetime = exampleClient.AuthorizationCodeLifetime,
                BackChannelLogoutSessionRequired = exampleClient.BackChannelLogoutSessionRequired,
                BackChannelLogoutUri = exampleClient.BackChannelLogoutUri,
                Claims = new List<IdentityServerClientClaim>(),
                ClientClaimsPrefix = exampleClient.ClientClaimsPrefix,
                ClientId = exampleClient.ClientId,
                ClientName = exampleClient.ClientName,
                ClientSecrets = new List<IdentityServerClientSecret>(),
                ClientUri = exampleClient.ClientUri,
                ConsentLifetime = exampleClient.ConsentLifetime,
                Description = exampleClient.Description,
                DeviceCodeLifetime = exampleClient.DeviceCodeLifetime,
                Enabled = exampleClient.Enabled,
                EnableLocalLogin = exampleClient.EnableLocalLogin,
                FrontChannelLogoutSessionRequired = exampleClient.FrontChannelLogoutSessionRequired,
                FrontChannelLogoutUri = exampleClient.FrontChannelLogoutUri,
                IdentityProviderRestrictions = new List<IdentityServerClientIdpRestriction>(),
                IdentityTokenLifetime = exampleClient.IdentityTokenLifetime,
                IncludeJwtId = exampleClient.IncludeJwtId,
                LogoUri = exampleClient.LogoUri,
                PairWiseSubjectSalt = exampleClient.PairWiseSubjectSalt,
                PostLogoutRedirectUris = new List<IdentityServerClientPostLogoutRedirectUri>(),
                ProtocolType = exampleClient.ProtocolType,
                RedirectUris = new List<IdentityServerClientRedirectUri>(),
                RefreshTokenExpiration = exampleClient.RefreshTokenExpiration,
                RefreshTokenUsage = exampleClient.RefreshTokenUsage,
                RequireClientSecret = exampleClient.RequireClientSecret,
                RequireConsent = exampleClient.RequireConsent,
                RequirePkce = exampleClient.RequirePkce,
                SlidingRefreshTokenLifetime = exampleClient.SlidingRefreshTokenLifetime,
                UpdateAccessTokenClaimsOnRefresh = exampleClient.UpdateAccessTokenClaimsOnRefresh,
                UserCodeType = exampleClient.UserCodeType,
                UserSsoLifetime = exampleClient.UserSsoLifetime
            };

            client.AllowedCorsOrigins.AddRange(exampleClient.AllowedCorsOrigins.Select(aco => new IdentityServerClientCorsOrigin { Origin = aco }));
            client.AllowedGrantTypes.AddRange(exampleClient.AllowedGrantTypes.Select(agt => new IdentityServerClientGrantType { GrantType = agt }));
            client.AllowedScopes.AddRange(exampleClient.AllowedScopes.Select(s => new IdentityServerClientScope { Scope = s }));
            client.Claims.AddRange(exampleClient.Claims.Select(s => new IdentityServerClientClaim { Type = s.Type, Value = s.Value }));
            client.ClientSecrets.AddRange(exampleClient.ClientSecrets.Select(s => new IdentityServerClientSecret { Description = s.Description, Expiration = s.Expiration, Type = s.Type, Value = s.Value }));
            client.IdentityProviderRestrictions.AddRange(exampleClient.IdentityProviderRestrictions.Select(s => new IdentityServerClientIdpRestriction { Provider = s }));
            client.PostLogoutRedirectUris.AddRange(exampleClient.PostLogoutRedirectUris.Select(s => new IdentityServerClientPostLogoutRedirectUri { PostLogoutRedirectUri = s }));

            _identityServerDbContext.IdentityServerClients.Add(client);

            await _identityServerDbContext.SaveChangesAsync();

        }

    }
}
