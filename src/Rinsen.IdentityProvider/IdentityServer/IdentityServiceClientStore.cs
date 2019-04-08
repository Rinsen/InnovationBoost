using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServiceClientStore : IClientStore
    {
        // https://id4withclients.readthedocs.io/en/latest/id4/ID4Database/DatabaseDiagramID4.html

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            switch (clientId)
            {
                case "mvc":
                    return Task.FromResult(new Client
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
                    });

                default:
                    break;
            }
            return Task.FromResult(new Client
            {
                ClientId = "client",

                // no interactive user, use the clientid/secret for authentication
                AllowedGrantTypes = GrantTypes.ClientCredentials,

                // secret for authentication
                ClientSecrets =
                {
                    new Secret("secret".Sha256())
                },

                // scopes that client has access to
                AllowedScopes = { "api1" }
            });
        }
    }
}
