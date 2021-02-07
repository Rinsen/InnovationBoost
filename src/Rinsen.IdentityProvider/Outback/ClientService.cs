using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.IdentityProvider.Outback.Entities;

namespace Rinsen.IdentityProvider.Outback
{
    public class ClientService
    {
        public Task<List<OutbackClient>> GetAll()
        {
            throw new NotImplementedException();
        }

        public Task<OutbackClient> GetClient(string id)
        {
            throw new NotImplementedException();
        }

        public Task DeleteIdentityServerClient(string id)
        {
            throw new NotImplementedException();
        }

        public Task<List<OutbackClientFamily>> GetAllClientFamilies()
        {
            throw new NotImplementedException();
        }

        public Task<OutbackClient> CreateNewClient(string clientId, string clientName, string description)
        {
            throw new NotImplementedException();
        }

        public Task<string> CreateNewTypedClient(string clientName, string description, string family)
        {
            throw new NotImplementedException();
        }

        public Task UpdateClient(OutbackClient client)
        {
            throw new NotImplementedException();
        }
    }
}
