using System.Collections.Generic;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResourceScope
    {
        public int Id { get; set; }

        public int ApiResourceId { get; set; }

        public string Name { get; set; }

        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Required { get; set; }

        public bool Emphasize { get; set; }

        public bool ShowInDiscoveryDocument { get; set; }

        public List<IdentityServerApiResourceScopeClaim> UserClaims { get; set; }
    }
}
