using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClientScope : ICreatedAndUpdatedTimestamp
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Scope { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }
    }
}
