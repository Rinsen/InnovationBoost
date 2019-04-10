using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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

    }
}
