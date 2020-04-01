using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.IdentityServer.Entities;

namespace Rinsen.IdentityProvider.IdentityServer
{
    public class IdentityServerDefaultInstaller
    {
        private readonly IdentityServerIdentityResourceBusiness _identityServerIdentityResourceBusiness;
        private readonly IdentityServerApiResourceBusiness _identityServerApiResourceBusiness;
        private readonly IdentityServerClientBusiness _identityServerClientBusiness;
        private readonly RandomStringGenerator _randomStringGenerator;

        public IdentityServerDefaultInstaller(IdentityServerIdentityResourceBusiness identityServerIdentityResourceBusiness,
            IdentityServerApiResourceBusiness identityServerApiResourceBusiness,
            IdentityServerClientBusiness identityServerClientBusiness,
            RandomStringGenerator randomStringGenerator)
        {
            _identityServerIdentityResourceBusiness = identityServerIdentityResourceBusiness;
            _identityServerApiResourceBusiness = identityServerApiResourceBusiness;
            _identityServerClientBusiness = identityServerClientBusiness;
            _randomStringGenerator = randomStringGenerator;
        }

        public async Task<Credentials> Install()
        {
            var identityResources = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourcesAsync();

            if (identityResources.Any())
            {
                return null;
            }

            await CreateOpenIdIdentityResource();
            await CreateProfileIdentityResource();
            await CreateInnovationBoostApiResource();
            var innovationBoostCredentials = await CreateInnovationBoostClient();

            return innovationBoostCredentials;
        }

        private async Task<Credentials> CreateInnovationBoostClient()
        {
            var id = await _identityServerClientBusiness.CreateNewTypedClient("InnovationBoost", "Innovation boost client", "WebApp");

            var client = await _identityServerClientBusiness.GetIdentityServerClient(id);

            var credentials = new Credentials
            {
                ClientId = client.ClientId,
                Secret = _randomStringGenerator.GetRandomString(40)
            };

            client.AllowedScopes.AddRange(new[]
            {
                new IdentityServerClientScope { Scope = "innovationboost.createlogs", State = ObjectState.Added },
            });

            client.AllowedGrantTypes.AddRange(new[]
            {
                new IdentityServerClientGrantType { GrantType = "client_credentials", State = ObjectState.Added }
            });

            client.ClientSecrets.AddRange(new[]
            {
                new IdentityServerClientSecret { Description = "Initial install secret", Type = "SharedSecret", Value = credentials.Secret, State = ObjectState.Added }
            });

            await _identityServerClientBusiness.UpdateClient(client);

            return credentials;
        }

        private async Task CreateOpenIdIdentityResource()
        {
            await _identityServerIdentityResourceBusiness.CreateNewIdentityResourceAsync("openid", "Your user identifier", "OpenId identity resource");

            var identityResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync("openid");
            
            identityResource.Claims.AddRange(new[]
            {
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "sub" },
            });

            await _identityServerIdentityResourceBusiness.UpdateIdentityResourceAsync(identityResource);
        }

        private async Task CreateProfileIdentityResource()
        {
            await _identityServerIdentityResourceBusiness.CreateNewIdentityResourceAsync("profile", "User profile", "Your user profile information (first name, last name, etc.)");

            var identityResource = await _identityServerIdentityResourceBusiness.GetIdentityServerIdentityResourceAsync("profile");
            
            identityResource.Claims.AddRange(new[]
            {
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "name" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "family_name" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "given_name" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "middle_name" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "nickname" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "preferred_username" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "profile" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "picture" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "website" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "gender" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "birthdate" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "zoneinfo" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "locale" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "updated_at" },
                new IdentityServerIdentityResourceClaim { State = ObjectState.Added, Type = "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name" }
            });

            await _identityServerIdentityResourceBusiness.UpdateIdentityResourceAsync(identityResource);
        }

        private async Task CreateInnovationBoostApiResource()
        {
            await _identityServerApiResourceBusiness.CreateNewApiResource("innovationboost", "Innovation Boost Api", "Api for innovation boost");

            var innovationBoostApiResource = await _identityServerApiResourceBusiness.GetIdentityServerApiResourceAsync("innovationboost");
            innovationBoostApiResource.Enabled = true;
            innovationBoostApiResource.State = ObjectState.Modified;

            innovationBoostApiResource.Scopes.AddRange(new[]
            {
                new IdentityServerApiResourceScope { Name = "innovationboost.createlogs", DisplayName = "Log service", Description = "Api for creating logs", State = ObjectState.Added },
                new IdentityServerApiResourceScope { Name = "innovationboost.sessions", DisplayName = "Session service", Description = "Api for managing sessions", State = ObjectState.Added },
                new IdentityServerApiResourceScope { Name = "innovationboost.sendmessage", DisplayName = "Send message service", Description = "Api for sending messages", State = ObjectState.Added },
            });

            await _identityServerApiResourceBusiness.UpdateApiResource(innovationBoostApiResource);
        }
    }

    public class Credentials
    {
        public string ClientId { get; set; }

        public string Secret { get; set; }

    }
}
