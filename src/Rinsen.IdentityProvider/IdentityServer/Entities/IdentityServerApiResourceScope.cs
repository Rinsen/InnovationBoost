using System;
using System.Collections.Generic;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResourceScope : ICreatedAndUpdatedTimestamp, IObjectWithState
    {
        public int Id { get; set; }

        public int ApiResourceId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ObjectState State { get; set; }

        public List<IdentityServerApiResourceScopeClaim> Claims { get; set; }
    }
}
