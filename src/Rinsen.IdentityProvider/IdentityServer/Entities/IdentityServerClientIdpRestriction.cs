using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerClientIdpRestriction : ICreatedAndUpdatedTimestamp
    {
        public int Id { get; set; }

        public int ClientId { get; set; }

        public string Provider { get; set; }
        public DateTimeOffset Created { get; set; }
        public DateTimeOffset Updated { get; set; }
    }
}
