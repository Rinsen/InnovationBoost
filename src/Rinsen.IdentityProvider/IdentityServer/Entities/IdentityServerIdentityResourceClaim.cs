using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerIdentityResourceClaim : ICreatedAndUpdatedTimestamp
    {
        public int Id { get; set; }

        public int IdentityResourceId { get; set; }

        public string Type { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

    }
}
