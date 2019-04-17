using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClientCorsOrigin : ICreatedAndUpdatedTimestamp
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Origin { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}
