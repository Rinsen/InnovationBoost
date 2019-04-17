using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
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

        public Task<IdentityServerClient> GetClient(string clientStringId)
        {
            throw new NotImplementedException();
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

            _identityServerDbContext.IdentityServerClients.Add(client);

            await _identityServerDbContext.SaveChangesAsync();

        }

    }
}
