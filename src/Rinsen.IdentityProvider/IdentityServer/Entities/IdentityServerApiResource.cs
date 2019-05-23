using System;
using System.Collections.Generic;
using System.Text;

namespace Rinsen.IdentityProvider.IdentityServer.Entities
{
    public class IdentityServerApiResource : ICreatedAndUpdatedTimestamp, IObjectWithState
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public string DisplayName { get; set; }

        public bool Enabled { get; set; }

        public string Name { get; set; }
        
        public DateTimeOffset Created { get; set; }

        public DateTimeOffset Updated { get; set; }

        public ObjectState State { get; set; }

        public List<IdentityServerApiResourceSecret> ApiSecrets { get; set; }
        
        public List<IdentityServerApiResourceScope> Scopes { get; set; }

        public List<IdentityServerApiResourceClaim> Claims { get; set; }

        public List<IdentityServerApiResourceProperty> Properties { get; set; }

    }
}
