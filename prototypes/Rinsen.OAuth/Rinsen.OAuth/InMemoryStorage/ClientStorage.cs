using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Clients;

namespace Rinsen.OAuth.InMemoryStorage
{
    public class ClientStorage : IClientAccessor
    {
        public Task<Client> GetClient(string clientId)
        {
            return Task.FromResult(GetClientPrivate(clientId));
        }

        private static Client GetClientPrivate(string clientId)
        {
            return new Client
            {
                AccessTokenLifetime = 3600,
                ClientId = clientId,
                ClientType = ClientType.Public,
                ClientClaims = new List<ClientClaim>(),
                ConsentRequired = false,
                IdentityTokenLifetime = 300,
                IssueRefreshToken = false,
                PostLogoutRedirectUris = new List<string>(),
                Secrets = new List<string>(),
                Scopes = new List<string>
                {
                    "openid",
                    "profile"
                },
                LoginRedirectUris = new List<string>
                {
                    "https://localhost:44372/signin-oidc"
                },
                SupportedGrantTypes = new List<string>
                {
                    "authorization_code"
                }
            };
        }

        //private static Client GetClientPrivate(string clientId)
        //{
        //    var secret = "fgdsnmkldfgdGDFGFghngj435";
        //    using var sha256 = SHA256.Create();
        //    var secretHash = WebEncoders.Base64UrlEncode(sha256.ComputeHash(Encoding.UTF8.GetBytes(secret)));
        //    return new Client
        //    {
        //        AccessTokenLifetime = 3600,
        //        ClientId = clientId,
        //        ClientType = ClientType.Confidential,
        //        ClientClaims = new List<ClientClaim>(),
        //        ConsentRequired = false,
        //        IdentityTokenLifetime = 300,
        //        IssueRefreshToken = false,
        //        PostLogoutRedirectUri = new List<string>(),
        //        Secrets = new List<string>
        //        {
        //            secretHash
        //        },
        //        Scopes = new List<string>
        //        {
        //            "openid",
        //            "profile"
        //        },
        //        LoginRedirectUris = new List<string>
        //        {
        //            "https://localhost:44372/signin-oidc"
        //        },
        //        SupportedGrantTypes = new List<string>
        //        {
        //            "authorization_code"
        //        }
        //    };
        //}
    }
}
