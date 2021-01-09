using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rinsen.Outback.Abstractons;
using Rinsen.Outback.Clients;

namespace Rinsen.IdentityProvider.Outback
{
    public class ClientAccessor : IClientAccessor
    {
        public Task<Client> GetClient(string clientId)
        {
            throw new NotImplementedException();
        }
    }
}
