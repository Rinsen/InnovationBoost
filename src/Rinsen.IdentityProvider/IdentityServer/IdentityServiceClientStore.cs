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
        private readonly IdentityServerClientBusiness _identityServerClient;

        public IdentityServiceClientStore(IdentityServerClientBusiness identityServerClient)
        {
            _identityServerClient = identityServerClient;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            return _identityServerClient.GetClient(clientId);
        }
    }
}
